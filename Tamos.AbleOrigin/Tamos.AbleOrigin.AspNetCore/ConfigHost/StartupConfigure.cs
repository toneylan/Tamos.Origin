using System.Reflection;
using Grpc.AspNetCore.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using ProtoBuf.Grpc.Server;
using SimpleInjector;
using Tamos.AbleOrigin.Booster;
using BadHttpRequestException = Microsoft.AspNetCore.Http.BadHttpRequestException;

namespace Tamos.AbleOrigin.AspNetCore;

public static class StartupConfigure
{
    #region Startup config

    // ReSharper disable once NotNullMemberIsNotInitialized
    private static Container _innerContainer;
    private static bool _isGrpcServer;

    /// <summary>
    /// 应用框架初始化，内部调用AddTamosFrame。Config to Asp.Net Core mode。
    /// <param name="frameConfig">Pre config TamosFrame，Notice：At this time, infra module eg. log and configuration has not init.</param>
    /// </summary>
    public static WebApplicationBuilder AddTamos(this WebApplicationBuilder builder, Action<TamosFrameOptions> frameConfig)
    {
        builder.Services.AddTamosFrame(tamosOpt =>
        {
            // Config frame in app
            tamosOpt.AddConfig(builder.Configuration);
            frameConfig(tamosOpt);
                
            // 配置IOC模式，即每个Request一个Scope。https://docs.simpleinjector.org/en/latest/aspnetintegration.html
            // AddAspNetCore() wraps web requests in a Simple Injector scope and allows request-scoped framework services to be resolved.
            _innerContainer = (Container)tamosOpt.InnerContainer;
            builder.Services.AddSimpleInjector(_innerContainer, simpOpt => simpOpt.AddAspNetCore());

            if (!IsPureGrpc())
            {
                // Add custom auth handle
                builder.Services.AddAuthentication(options =>
                {
                    options.AddScheme<PassportAuthHandler>(PassportAuthHandler.SchemeName, "Passport Scheme");
                    options.DefaultAuthenticateScheme = PassportAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = PassportAuthHandler.SchemeName;
                });
            }
        });
            
        //CrossWire .Net DI https://docs.simpleinjector.org/en/latest/servicecollectionintegration.html
        //services.AddHttpClient();

        return builder;
    }

    /// <summary>
    /// Tamos框架下的默认配置，调用后才能从IOC获取实例。<br/>
    /// UseSimpleInjector<br/>
    /// AddExceptionHandler<br/>
    /// UseAuthentication [NotGrpcServer]<br/>
    /// + Application stop处理
    /// </summary>
    public static WebApplication UseTamos(this WebApplication app, Action? configAction = null)
    {
        // finalizes the integration process.
        (app as IApplicationBuilder).UseSimpleInjector(_innerContainer);
        app.Services.UseProvider();

        if (CentralConfiguration.IsEnvDev()) app.UseDeveloperExceptionPage();
        app.AddExceptionHandler();

        //应用配置
        configAction?.Invoke();

        //app.UseRouting();

        if (!IsPureGrpc())
        {
            app.UseAuthentication();
            app.UseAuthorization(); //启用后，Authorize验证才有效
        }

        //docker里不能读取输入，否则会导致app直接退出！
        if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
        {
            Task.Run(() =>
            {
                var recMsg = Console.ReadLine();
                Console.WriteLine("StopApplication by console input:" + recMsg);
                app.Lifetime.StopApplication();
            });
        }

        return app;
    }
        
    /// <summary>
    /// Config the Tamos Passport module.
    /// </summary>
    public static void ConfigPassport(this TamosFrameOptions _, Action<PassportOptions> optAction)
    {
        optAction(PassportOptions.Current);
    }

    #endregion

    #region Grpc server

    /// <summary>
    /// 是否仅为Grpc Server，目前为了辨识iBos同时有Web Api。
    /// </summary>
    private static bool IsPureGrpc()
    {
        return _isGrpcServer && !HostApp.Options.IsStandalone; //IsStandalone for iBos
    }

    /// <summary>
    /// 配置Grpc服务。提供defaultAddress，则按GetAddressByDeployName配置监听Http2。
    /// </summary>
    public static void AddGrpcServer(this WebApplicationBuilder builder, string? defaultAddress, Action<GrpcServiceOptions>? optConfig = null)
    {
        _isGrpcServer = true;

        if (defaultAddress.NotNull())
        {
            builder.WebHost.ConfigureKestrel(options =>
            {
                var hostAddress = ServiceAddressConfig.GetDeployInstanceAddress(defaultAddress);
                var (_, port) = RpcFacade.ParseAddress(hostAddress);
                //监听所有地址
                options.ListenAnyIP(port, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
                LogService.DebugFormat("Grpc server listening on:{0}", port);
            });
        }

        builder.Services.AddCodeFirstGrpc(config =>
        {
            config.EnableDetailedErrors = true;
            config.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Optimal;

            optConfig?.Invoke(config);
        });
    }
        
    #endregion

    #region Web api

    /// <summary>
    /// 添加WebApi设置：保持属性原始命名，自定义DateTime Converter。
    /// </summary>
    public static IMvcBuilder AddWebApiSet(this IServiceCollection services)
    {
        return services.AddControllers(options =>
            {
                if (CentralConfiguration.IsEnvDev())
                {
                    options.Filters.Add(new ProducesAttribute("application/json"));
                    options.Filters.Add(new ConsumesAttribute("application/json"));
                }
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
            });
    }

    #endregion

    #region UseStaticFile

    /// <summary>
    /// 添加Spa前端路径，同时将Router路径（404）重定向到index.html。<br/>
    /// requestPath默认为：/<paramref name="relSpaPath"/>
    /// </summary>
    public static WebApplication UseSpaPath(this WebApplication app, string relSpaPath)
    {
        if (!Directory.Exists(HostApp.GetPath(relSpaPath))) return app;

        var requestPath = $"/{relSpaPath}";
        app.UseWhen(context => context.Request.Path.StartsWithSegments(requestPath) && !Path.HasExtension(context.Request.Path.Value),
            builder => builder.Use(async (context, next) =>
            {
                context.Request.Path = requestPath + "/"; //重定向到index.html
                await next();

                /*context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync(Path.Combine(HostApp.RootPath, "we/index.html"));*/
            }));
            
        //注意：该静态文件注册要放后边，才能保证在Http pipeline中，处理重写后的 Request Path。
        return app.UseStaticFileDir(relSpaPath);
    }

    /// <summary>
    /// 添加静态文件目录与默认文档，否则无法Web访问。<br/>
    /// requestPath默认为：/{relDirPath}
    /// </summary>
    public static WebApplication UseStaticFileDir(this WebApplication app, string relDirPath, string? requestPath = null)
    {
        //** Docker运行后，新增如www目录，可能引起运行出错。（应是目录bind的Bug）
        var dirPath = HostApp.GetPath(relDirPath);
        if (Directory.Exists(dirPath))
        {
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(dirPath),
                RequestPath = requestPath ?? $"/{relDirPath}",
            });
        }

        return app;
    }

    #endregion

    #region Exception handle
        
    /// <summary>
    /// 自定义的异常处理
    /// </summary>
    internal static IApplicationBuilder AddExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                await LogException(context, exPathFeature);

                await context.Response.WriteAsync(exPathFeature.Error.Message);
            });
        });

        return app;
    }

    private static async Task LogException(HttpContext context, IExceptionHandlerPathFeature? exPathFeature)
    {
        if (exPathFeature == null) return;

        //异常类型
        if (exPathFeature.Error is BadHttpRequestException badReq)
        {
            try
            {
                //context.Request.EnableBuffering();
                var body = await context.Request.ReadBody();

                LogService.ErrorFormat("{0} BadRequest：{1}\n{2}", exPathFeature.Path, badReq.Message, context.Request.DumpRequest(body));
            }
            catch (Exception e)
            {
                LogService.ErrorFormat("Dump BadRequest error:{0}", e);
            }
        }
        else LogService.ErrorFormat("{0}未处理异常：{1}", exPathFeature.Path, exPathFeature.Error);
                    
        //开发环境时抛出异常，前台才能看到异常详情（UseDeveloperExceptionPage）
        if (CentralConfiguration.IsEnvDev())
        {
            throw exPathFeature.Error;
        }
    }

    #endregion

    #region Swagger

    /// <summary>
    /// 自定义Swagger中Action的Tag（分组）。
    /// </summary>
    public static IList<string> SwaggerTagAction(ApiDescription api)
    {
        if (api.ActionDescriptor is not ControllerActionDescriptor actionDescriptor) return new[] { api.GroupName };

        var setting = actionDescriptor.ControllerTypeInfo.GetCustomAttribute<OpenApiSettingAttribute>();
        return new[] { setting?.Tag ?? actionDescriptor.ControllerName };
    }

    #endregion
}
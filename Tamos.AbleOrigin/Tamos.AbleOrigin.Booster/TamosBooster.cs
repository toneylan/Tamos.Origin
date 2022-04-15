using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.Booster
{
    /// <summary>
    /// Tamos框架的配置与注册，Net core 风格的服务配置。
    /// </summary>
    public static class TamosBooster
    {
        /// <summary>
        /// 添加Tamos框架，完成之后，日志等基础功能服务都已注册。
        /// </summary>
        /// <param name="services"></param>
        /// <param name="frameConfig">Frame预配置，如在Asp.net core中将容器配置为Web模式。注意：是在日志等基础模块初始化前调用的</param>
        public static IServiceCollection AddTamosFrame(this IServiceCollection services, Action<TamosFrameOptions> frameConfig)
        {
            //IOC init
            var serviceContainer = new SimpleServiceContainer();
            ServiceLocator.UseContainer(serviceContainer);

            // Frame 配置
            var frameOptions = new TamosFrameOptions
            {
                InnerContainer = serviceContainer.Container
            };
            frameConfig.Invoke(frameOptions);

            //-- SimpleInjector调用GetInstance后，就无法再Register！这里只能手动创建。
            LogService.SetProvider(new Log4netIntegration(HostApp.Options.AppName));
            CentralConfiguration.SetProvider<ConsulConfigProvider>(new AppSettingProvider());
            EntMapper.SetProvider(new ObjMapster());

            //-- Reg basic func implement.
            serviceContainer.Register<IJsonSerializer, JsonSerializer>(LifeStyleType.Singleton);
            if (!HostApp.Options.IsStandalone)
            {
                serviceContainer.Register<ICacheProvider, RedisCache>(LifeStyleType.Singleton);
                serviceContainer.Register<IDistributedSrvProvider, RedisDistSrvProvider>(LifeStyleType.Singleton);
                serviceContainer.AddMQProvider<RabbitMQProvider>();

                //不是Dev环境则启用集中日志（因为读取了配置，故放在配置初始化后执行）
                if (!CentralConfiguration.IsEnvDev()) Log4netIntegration.AddCentralLog();
            }
            serviceContainer.Register<RpcChannelFactory, GrpcChannelFactory>(LifeStyleType.Singleton);
            
            // App config
            if (frameOptions.AppSrvConfig != null)
            {
                frameOptions.AppSrvConfig(serviceContainer);
                frameOptions.AppSrvConfig = null; //release ref
            }

            return services;
        }
    }

    #region TamosFrameOptions

    public sealed class TamosFrameOptions
    {
        internal Action<IServiceContainer>? AppSrvConfig;

        /// <summary>
        /// IOC内部的Container，目前是SimpleInjector.Container
        /// </summary>
        public IServiceProvider InnerContainer { get; init; }

        /// <summary>
        /// 配置App信息。
        /// </summary>
        public void ConfigApp(string appName)
        {
            HostApp.Options.AppName = appName;
        }

        /// <summary>
        /// 添加本地配置，可通过AppSetting获取
        /// </summary>
        public void AddConfig(IConfiguration configuration)
        {
            AppSettingProvider.UseConfiguration(configuration);
        }

        /// <summary>
        /// Register App service config action, it will be called after Tamos frame initialized.
        /// </summary>
        public void ConfigAppService(Action<IServiceContainer> configAction)
        {
            AppSrvConfig = configAction;
        }
    }

    #endregion
}
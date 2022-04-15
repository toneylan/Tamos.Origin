/*
 * 共享的Application配置代码，谨慎修改。
 */
#if DEBUG
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Tamos.AbleOrigin.AspNetCore;

public static class ProgramPart
{
    internal static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = HostApp.Options.AppName, Version = "v1" });
            options.TagActionsBy(StartupConfigure.SwaggerTagAction);
            options.CustomOperationIds(api => api.ActionDescriptor is ControllerActionDescriptor action ? action.ControllerName + action.ActionName : null);
            options.AddServer(new OpenApiServer { Url = "/" });
            //options.UseAllOfForInheritance(); 开启后OpenApi generator也不支持，且会生成警告信息。
        });
        // xml comments
        /*var xmlFilePath = HostApp.GetPath("Aotran.IntelBiz.Core.xml");
        c.IncludeXmlComments(xmlFilePath1);*/
    }

    internal static void UseSwaggerDev(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", c.DocumentTitle + " v1"));
    }
}
#endif
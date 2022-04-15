using System.IO;
using Microsoft.Extensions.Configuration;

namespace Tamos.AbleOrigin.Booster
{
    internal class AppSettingProvider : IAppSettingProvider
    {
        //private const string ConfSetFile = "appsettings.json";
        private static IConfiguration? _configuration;

        internal static void UseConfiguration(IConfiguration config)
        {
            _configuration = config;
        }

        #region Implementation of IAppSettingProvider
        
        public string? GetAppSetting(string key)
        {
            return _configuration?[key];
        }

        internal static string? InnerGetAppSet(string key)
        {
            return _configuration?[key];

            /*var builder = new ConfigurationBuilder().SetBasePath(HostApp.RootPath);
            var setFile = HostApp.GetPath(ConfSetFile);
            if (File.Exists(setFile)) builder.AddJsonFile(setFile, optional: true, reloadOnChange: true);
            else LogService.WarnFormat($"缺少本地配置文件{ConfSetFile}");
                
            //.AddEnvironmentVariables();
            _configuration = builder.Build();

            return _configuration[key];*/
        }

        #endregion
    }
}
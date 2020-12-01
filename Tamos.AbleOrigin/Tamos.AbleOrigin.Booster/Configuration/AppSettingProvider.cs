using System.IO;
using Microsoft.Extensions.Configuration;
using Tamos.AbleOrigin.Configuration;
using Tamos.AbleOrigin.Log;

namespace Tamos.AbleOrigin.Booster
{
    internal class AppSettingProvider : IAppSettingProvider
    {
        private const string ConfSetFile = "appsettings.json";
        private static IConfigurationRoot _configuration;

        #region Implementation of IAppSettingProvider
        
        public string GetAppSetting(string key)
        {
            return InnerGetAppSet(key);
        }

        internal static string InnerGetAppSet(string key)
        {
            if (_configuration == null)
            {
                var builder = new ConfigurationBuilder().SetBasePath(HostApp.RootPath);
                var setFile = HostApp.GetPath(ConfSetFile);
                if (File.Exists(setFile)) builder.AddJsonFile(setFile, optional: false, reloadOnChange: true);
                else LogService.WarnFormat($"缺少本地配置文件{ConfSetFile}");
                
                //.AddJsonFile($"appsettings.{EnvironmentName}.json", optional: true, reloadOnChange: true)
                //.AddEnvironmentVariables();
                _configuration = builder.Build();
            }

            return _configuration[key];
        }

        #endregion
    }
}
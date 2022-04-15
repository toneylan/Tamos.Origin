using System;
using System.Diagnostics.CodeAnalysis;

namespace Tamos.AbleOrigin
{
    /// <summary>
    /// 统一集中的配置管理，所有设置都会以环境（Environment），作为根目录划分。<br />
    /// 环境有两种设置方式，1、本地配置文件中设置"DeployEnvironment"；2、操作系统环境变量设置"TamosEnv"。<br />
    /// 环境设置值参考：Dev、Production。
    /// </summary>
    public static class CentralConfiguration
    {
        private const string Env_Dev = "Dev";
        //private const string Env_Test = "Test";
        private const string Env_Production = "Production";

        #region ConfigProvider
        
        private static ICentralConfigProvider _configProvider;
        // ReSharper disable once ConstantNullCoalescingCondition
        private static ICentralConfigProvider ConfigProvider => _configProvider ??=
            (ServiceLocator.GetOrReflect<ICentralConfigProvider>("ConsulConfigProvider") ?? new NullConfigProvider());

        //-- 本地配置文件设置
        private static IAppSettingProvider _appSetProvider;

        // ReSharper disable once ConstantNullCoalescingCondition
        private static IAppSettingProvider AppSetProvider => _appSetProvider ??= ServiceLocator.GetOrReflect<IAppSettingProvider>("AppSettingProvider");

        /// <summary>
        /// 设置Provider并初始化环境设置，由Booster调用。
        /// </summary>
        public static void SetProvider<T>(IAppSettingProvider appSetProvider) where T : ICentralConfigProvider, new()
        {
            //先初始化AppSet，因为要从本地检查环境设置。
            _appSetProvider = appSetProvider;
            InitEnvironment();

            if (!HostApp.Options.IsStandalone) _configProvider = new T(); //再设置集中配置Provider
        }

        //环境设置
        private static void InitEnvironment()
        {
            try
            {
                DeployEnv = GetAppSetting("DeployEnvironment")!;
                if (!string.IsNullOrEmpty(DeployEnv)) return;

                //检查系统环境变量设置，否则默认使用生产环境。
                var computerSet = Environment.GetEnvironmentVariable("TAMOS_ENV") ?? Environment.GetEnvironmentVariable("TAMOS_ENV", EnvironmentVariableTarget.User);
                DeployEnv = !string.IsNullOrEmpty(computerSet) ? computerSet : Env_Production;
            }
            catch (Exception e)
            {
                LogService.SafeWrite(e.ToString());
            }
        }

        #endregion

        /// <summary>
        /// 当前部署的环境，如：Dev、Test、Production（开发、测试、生产）
        /// </summary>
        public static string DeployEnv { get; private set; }
        
        #region Central Set & Get

        /// <summary>
        /// 设置K/V值，key可用"/"划分目录，如：WebSites/Portal
        /// </summary>
        public static bool Set(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) return false;

            try
            {
                return ConfigProvider.Set($"{DeployEnv}/{key}", value);
            }
            catch (Exception e)
            {
                LogService.Error(e);
                return false;
            }
        }

        /// <summary>
        /// 获取K/V值，key可包含"/"划分的目录
        /// </summary>
        [return: NotNullIfNotNull("defaultVal")]
        public static string? Get(string key, string? defaultVal = null)
        {
            if (string.IsNullOrEmpty(key)) return defaultVal;

            try
            {
                return ConfigProvider.Get($"{DeployEnv}/{key}") ?? defaultVal;
            }
            catch (Exception e)
            {
                LogService.Error(e);
                return defaultVal;
            }
        }

        #endregion

        #region AppSetting

        /// <summary>
        /// 获取程序本地（appsettings.json）的配置
        /// </summary>
        public static string? GetAppSetting(string key, string? defaultVal = null)
        {
            if (string.IsNullOrEmpty(key)) return defaultVal;

            try
            {
                return AppSetProvider.GetAppSetting(key) ?? defaultVal;
            }
            catch (Exception e)
            {
                LogService.Error(e);
                return defaultVal;
            }
        }

        #endregion

        #region Check Deploy Env

        /// <summary>
        /// 是否处于开发环境
        /// </summary>
        public static bool IsEnvDev()
        {
            return DeployEnv == Env_Dev;
        }

        #endregion
    }
}
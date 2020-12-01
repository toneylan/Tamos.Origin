using System;
using System.Collections.Generic;
using Tamos.AbleOrigin.Cache;
using Tamos.AbleOrigin.Configuration;
using Tamos.AbleOrigin.IOC;
using Tamos.AbleOrigin.Log;
using Tamos.AbleOrigin.Mapper;
using Tamos.AbleOrigin.Serialize;

namespace Tamos.AbleOrigin.Booster
{
    /// <summary>
    /// 系统框架的初始化类，在应用程序启动时调用Initialize方法
    /// </summary>
    public static class FrameInitializer
    {
        /// <summary>
        /// 框架初始化入口
        /// </summary>
        /// <param name="serviceInstall"></param>
        /// <param name="appConfig">注意：是在日志和配置模块初始化前调用的</param>
        public static void Initialize(Action<IServiceContainer> serviceInstall, Action<HostAppOptions> appConfig = null)
        {
            //IOC init
            var serviceContainer = new SimpleServiceContainer();
            ServiceLocator.SetServiceContainer(serviceContainer);

            //App中配置
            _preConfigAction?.Invoke(serviceContainer.Container);
            appConfig?.Invoke(HostApp.Options);

            //SimpleInjector调用GetInstance后，就无法再Register！这里只能手动创建。
            LogService.SetProvider(new Log4netIntegration(HostApp.Options.AppName));
            CentralConfiguration.SetProvider<ConsulConfigProvider>(new AppSettingProvider());
            EntMapper.SetProvider(new ObjMapster());

            //Reg basic func implement.
            serviceContainer.Register<IJsonSerializer, JsonSerializer>(LifeStyleType.Singleton);
            if (!HostApp.Options.IsStandalone)
            {
                serviceContainer.Register<ICacheProvider, RedisCache>(LifeStyleType.Singleton);
                serviceContainer.Register<IDistributedSrvProvider, RedisDistSrvProvider>(LifeStyleType.Singleton);

                //不是Dev环境则启用集中日志（因为读取了配置，故放在配置初始化后执行）
                if (!CentralConfiguration.IsEnvDev()) Log4netIntegration.AddCentralLog();
            }

            //最后执行应用初始化，确保日志、配置等实现都已注册！
            serviceInstall?.Invoke(serviceContainer);
        }


        #region Reg Configure

        private static Action<IServiceProvider> _preConfigAction;

        /// <summary>
        /// 开始服务注册前的配置。如在应用中将容器配置为Web模式。configAction在Initialize中调用。
        /// </summary>
        public static void PreConfigure(Action<IServiceProvider> configAction)
        {
            _preConfigAction = configAction;
        }

        #endregion

        #region Release work

        private static readonly List<IDisposable> DisposeList = new List<IDisposable>();

        /// <summary>
        /// 程序退出事件，注册退出时行为
        /// </summary>
        public static event Action AppExiting;

        /// <summary>
        /// 注册Release时，要释放的对象
        /// </summary>
        public static T RegDisposeObj<T>(T obj) where T : IDisposable
        {
            DisposeList.Add(obj);
            return obj;
        }

        /// <summary>
        /// Release domain source
        /// </summary>
        public static void Release()
        {
            try
            {
                AppExiting?.Invoke();

                foreach (var item in DisposeList)
                {
                    item.Dispose();
                }

                ServiceLocator.Container?.Dispose();
            }
            catch (Exception e)
            {
                LogService.Error(e);
            }
        }

        #endregion
    }
}
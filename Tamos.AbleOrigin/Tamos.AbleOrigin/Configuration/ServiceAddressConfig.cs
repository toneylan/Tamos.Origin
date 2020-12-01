using Tamos.AbleOrigin.Common;

namespace Tamos.AbleOrigin.Configuration
{
    /// <summary>
    /// 服务地址配置读取
    /// </summary>
    public static class ServiceAddressConfig
    {
        private const string Catalog = "ServiceAddress";
        private const string ExternalCatalog = "ExternalService";

        #region App service
        
        /// <summary>
        /// 获取当前部署的服务名称，同一服务部署多个实例时，保持唯一，如：MimsDataBroker1，MimsDataBroker2
        /// </summary>
        public static string GetDeployServiceName(string defaultName)
        {
            return CentralConfiguration.GetAppSetting("DeployServiceName", defaultName);
        }

        /// <summary>
        /// 通过服务名获取地址。（Consul目录：ServiceAddress）
        /// </summary>
        public static string GetAddress(string srvName, string defaultAddress, bool readAppSetting = false)
        {
            var catKey = $"{Catalog}/{srvName}";
            return readAppSetting
                ? CentralConfiguration.GetAppSetting(srvName, CentralConfiguration.Get(catKey, defaultAddress))
                : CentralConfiguration.Get(catKey, defaultAddress);
        }

        /// <summary>
        /// 通过部署的服务名，获取服务地址配置
        /// </summary>
        public static string GetAddressByDeployName(string defaultAddress)
        {
            var depSrvName = GetDeployServiceName(null);
            if (string.IsNullOrEmpty(depSrvName)) return defaultAddress;

            return GetAddress(depSrvName, defaultAddress);
        }

        #endregion

        /// <summary>
        /// 获取外部服务设置，如MQ、缓存等。（Consul目录：ExternalService）。<br />
        /// readAppSetting 是否优先读取本地配置。
        /// </summary>
        public static string GetExternalSrvSet(string key, string defaultSet = null, bool readAppSetting = false)
        {
            if (readAppSetting)
            {
                var localSet = CentralConfiguration.GetAppSetting(key);
                if (localSet.NotNull()) return localSet;
            }
            return CentralConfiguration.Get($"{ExternalCatalog}/{key}", defaultSet);
        }
    }
}
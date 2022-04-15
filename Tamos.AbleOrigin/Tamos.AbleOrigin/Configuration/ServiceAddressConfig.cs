using System.Diagnostics.CodeAnalysis;

namespace Tamos.AbleOrigin
{
    /// <summary>
    /// 服务地址配置读取
    /// </summary>
    public class ServiceAddressConfig : BaseCatalogConfig<ServiceAddressConfig>
    {
        private const string CurCatalog = "ServiceAddress";
        protected override string Catalog => CurCatalog;

        private const string ExternalCatalog = "ExternalService";

        #region App service

        /// <summary>
        /// 获取部署的实例名称，如Cluster模式多实例时：central-svc1，central-svc2 ……
        /// </summary>
        public static string? GetDeployInstanceName(string? defaultName)
        {
            return CentralConfiguration.GetAppSetting("DeployInstanceName", defaultName);
        }

        /// <summary>
        /// 按服务名获取地址。配置目录：ServiceAddress
        /// </summary>
        public static string GetAddress(string svcName, string defaultAddress) //, bool readAppSetting = false
        {
            var setKey = $"{CurCatalog}/{svcName}";

            //(readAppSetting ? CentralConfiguration.GetAppSetting(svcName, CentralConfiguration.Get(setKey, defaultAddress)) : null) ??
            return CentralConfiguration.Get(setKey, defaultAddress);
        }

        /// <summary>
        /// 通过当前实例名称，获取对应的地址配置。
        /// </summary>
        //[return: NotNullIfNotNull("defaultAddress")]
        public static string GetDeployInstanceAddress(string defaultAddress)
        {
            var depInsName = GetDeployInstanceName(null);
            return depInsName.IsNull() ? defaultAddress : GetAddress(depInsName, defaultAddress);
        }

        #endregion

        /// <summary>
        /// 获取外部服务配置，如MQ、缓存等中间件。配置目录：ExternalService<br />
        /// </summary>
        [return: NotNullIfNotNull("defaultSet")]
        public static string? GetExternalSvcSet(string key, string? defaultSet = null) //readAppSetting 是否优先读取本地配置。
        {
            /*if (readAppSetting)
            {
                var localSet = CentralConfiguration.GetAppSetting(key);
                if (localSet.NotNull()) return localSet;
            }*/

            return CentralConfiguration.Get($"{ExternalCatalog}/{key}", defaultSet);
        }
    }
}
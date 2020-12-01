namespace Tamos.AbleOrigin.Payment.WxPay
{
    internal static class WxObjExtend
    {
        /// <summary>
        /// 非空的值才会被设置
        /// </summary>
        public static void Set(this WxPayData data, string key, string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            data.SetValue(key, value);
        }

        public static string GetValueStr(this WxPayData data, string key)
        {
            return data.GetValue(key)?.ToString();
        }
    }

    #region WxPayConfig

    /// <summary>
    /// 配置账号信息
    /// </summary>
    internal class WxPayConfig : IConfig
    {
        private readonly CommonPayConfig _config;

        public WxPayConfig(CommonPayConfig config)
        {
            _config = config;
        }


        #region Implementation of IConfig

        public string GetAppID()
        {
            return _config.AppId;
        }

        public string GetMchID()
        {
            return _config.MerchantAccount;
        }

        public string GetKey()
        {
            return _config.AppKey;
        }

        public string GetAppSecret()
        {
            return _config.ExtPrivateKey;
        }

        public string GetSSlCertPath()
        {
            return _config.ApiCertPath;
        }

        public string GetSSlCertPassword()
        {
            return _config.ApiCertPwd ?? _config.MerchantAccount;
        }

        public string GetNotifyUrl()
        {
            return _config.NotifyUrl;
        }

        public string GetIp()
        {
            return "127.0.0.1";
        }

        public string GetProxyUrl()
        {
            return string.Empty;
        }

        public int GetReportLevel()
        {
            return 1;
        }

        public int GetLogLevel()
        {
            return 3;
        }

        #endregion
    }

    #endregion


}
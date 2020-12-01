namespace Tamos.AbleOrigin.Payment
{
    public class CommonPayConfig
    {
        /// <summary>
        /// 支付应用标识
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 交易过程生成签名的密钥
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// 商户/卖家账户
        /// </summary>
        public string MerchantAccount { get; set; }

        /// <summary>
        /// 服务商模式的子应用Id
        /// </summary>
        public string SubAppId { get; set; }

        /// <summary>
        /// 服务商模式的子商户
        /// </summary>
        public string SubMerAccount { get; set; }

        /// <summary>
        /// 额外私钥
        /// </summary>
        public string ExtPrivateKey { get; set; }

        /// <summary>
        /// 证书绝对路径（如微信支付退款、撤销订单时需要）
        /// </summary>
        public string ApiCertPath { get; set; }

        /// <summary>
        /// 证书密码
        /// </summary>
        public string ApiCertPwd { get; set; }

        /// <summary>
        /// 支付结果通知回调url（通常是支付平台异步通知）
        /// </summary>
        public string NotifyUrl { get; set; }   
    }
}
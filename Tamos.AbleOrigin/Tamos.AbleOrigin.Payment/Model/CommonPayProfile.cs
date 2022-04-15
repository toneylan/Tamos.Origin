using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.Payment
{
    public class CommonPayProfile
    {
        public PayGatewayType GatewayType { get; set; }

        /// <summary>
        /// 支付应用标识
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 交易过程的加密密钥
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// 支付平台的商户/卖家账户
        /// </summary>
        public string? MerchantId { get; set; }

        /// <summary>
        /// 服务商模式的子应用Id
        /// </summary>
        public string? SubAppId { get; set; }

        /// <summary>
        /// 服务商模式的子商户
        /// </summary>
        public string? SubMerchantId { get; set; }

        /*/// <summary>
        /// 额外私钥
        /// </summary>
        public string? ExtPrivateKey { get; set; }*/
        
        /// <summary>
        /// 支付结果通知回调url（通常是支付平台异步通知）
        /// </summary>
        public string? NotifyUrl { get; set; }   
    }
}
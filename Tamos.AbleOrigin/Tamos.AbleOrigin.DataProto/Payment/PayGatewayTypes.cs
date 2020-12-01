using System.ComponentModel;

namespace Tamos.AbleOrigin.DataProto.Payment
{
    /// <summary>
    /// 支付渠道枚举
    /// </summary>
    public enum PayGatewayTypes
    {
        /*/// <summary>
        /// 支付宝条码支付
        /// </summary>
        [Description("支付宝条码支付")]
        AliBarcodePay = 3,*/

        /// <summary>
        /// 微信付款码支付
        /// </summary>
        [Description("微信付款码支付")]
        WxMicroPay = 6,

        /// <summary>
        /// 微信内支付
        /// </summary>
        [Description("微信内支付")]
        WxJsApiPay = 8,

        /// <summary>
        /// Cba云支付
        /// </summary>
        [Description("Cba云支付")]
        CbaCloudPay = 51
    }
}
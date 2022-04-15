using System.ComponentModel;

namespace Tamos.AbleOrigin.DataProto
{
    /// <summary>
    /// 支付渠道枚举
    /// </summary>
    public enum PayGatewayType
    {
        /// <summary>
        /// 未设置
        /// </summary>
        [Description("未设置")]
        None = 0,
        
        /// <summary>
        /// 微信付款码
        /// </summary>
        [Description("微信付款码")]
        WxMicroPay = 51,
        
        /// <summary>
        /// 微信内支付（公众号支付）
        /// </summary>
        [Description("微信内支付")]
        WxJsApi = 52,

        /// <summary>
        /// 收钱吧B扫C
        /// </summary>
        [Description("收钱吧B扫C")]
        SqbPayCode = 81,

        /// <summary>
        /// 收钱吧wap
        /// </summary>
        [Description("收钱吧Wap")]
        SqbWap = 85,
    }
}
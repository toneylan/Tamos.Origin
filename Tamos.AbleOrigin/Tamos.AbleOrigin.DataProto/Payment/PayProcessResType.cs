namespace Tamos.AbleOrigin.DataProto
{
    /// <summary>
    /// 支付处理的结果类型
    /// </summary>
    public enum PayProcessResType
    {
        /// <summary>
        /// 支付失败
        /// </summary>
        Error = 0,

        /// <summary>
        /// 请求成功（如微信统一下单），可进入后续支付流程
        /// </summary>
        RequestSuccess = 1,
        
        /// <summary>
        /// 支付成功（如付款码直接支付）
        /// </summary>
        PaymentSuccess = 10
    }

    /// <summary>
    /// 支付结果的后续处理方式
    /// </summary>
    public enum PayResHandleWay
    {
        None = 0,
        
        /// <summary>
        /// 执行跳转，如到支付网关或三方支付平台
        /// </summary>
        Redirect = 2
    }
}
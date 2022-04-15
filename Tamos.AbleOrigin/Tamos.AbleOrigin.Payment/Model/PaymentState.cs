using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.Payment
{
    /// <summary>
    /// 指示支付状态信息
    /// </summary>
    internal enum PaymentState
    {
        /// <summary>
        /// 支付失败
        /// </summary>
        PayFail = PayProcessResType.Error,

        /// <summary>
        /// 等待支付中，通常需查询，直到获取到最终状态。
        /// </summary>
        WaitPaying = 1,
        
        /// <summary>
        /// 已支付成功
        /// </summary>
        PaymentSuccess = PayProcessResType.PaymentSuccess,

        
    }
}
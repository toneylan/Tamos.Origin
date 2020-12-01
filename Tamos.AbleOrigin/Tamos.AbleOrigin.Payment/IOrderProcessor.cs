using Tamos.AbleOrigin.DataProto.Payment;

namespace Tamos.AbleOrigin.Payment
{
    /// <summary>
    /// 定义支付相关的订单处理方法
    /// </summary>
    public interface IOrderProcessor
    {
        /// <summary>
        /// 该接口会在具体应用系统里实现，当支付回调被确认为付款成功时，会自动调用该接口，以执行具体的支付成功逻辑。
        /// </summary>
        OrderProcessResult OnOrderPaid(OrderProcessPara para);
    }
}
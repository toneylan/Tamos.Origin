using System.Runtime.Serialization;

namespace Tamos.AbleOrigin.DataProto.Payment
{
    /// <summary>
    /// 支付通知的处理结果
    /// </summary>
    [DataContract]
    public class NotifyProcessResult : IGeneralResObj
    {
        /*[DataMember(Order = 1)]
        public PayProcessResType ResultType { get; set; }*/

        [DataMember(Order = 2)]
        public string TransactionId { get; set; }

        /// <summary>
        /// 支付渠道方的流水号
        /// </summary>
        [DataMember(Order = 3)]
        public string GatewayTranId { get; set; }

        //public string OrderId { get; set; }

        /// <summary>
        /// 错误描述信息
        /// </summary>
        [DataMember(Order = 20)]
        public string ErrorMsg { get; set; }

        /*/// <summary>
        /// 是否已成功支付
        /// </summary>
        public bool IsPaymentSuccess => ResultType == PayProcessResType.PaymentSuccess;*/
    }
}
using System.Runtime.Serialization;

namespace Tamos.AbleOrigin.DataProto.Payment
{
    /// <summary>
    /// 处理支付交易的结果
    /// </summary>
    [DataContract]
    public class PayProcessResult : IGeneralResObj
    {
        [DataMember(Order = 1)]
        public PayProcessResType ResultType { get; set; }

        /// <summary>
        /// 支付渠道的流水号。WxJsApi等可能不会返回。
        /// </summary>
        [DataMember(Order = 2)]
        public string GatewayTranId { get; set; }

        /// <summary>
        /// 处理结果信息。如用于后续支付的参数、跳转的url等
        /// </summary>
        [DataMember(Order = 3)]
        public string ResultValue { get; set; }

        /// <summary>
        /// 附加的结果数据
        /// </summary>
        [DataMember(Order = 4)]
        public string AttachData { get; set; }

        /// <summary>
        /// 后续处理方式
        /// </summary>
        [DataMember(Order = 5)]
        public PayResHandleWay ResHandleWay { get; set; }

        /// <summary>
        /// 错误描述信息
        /// </summary>
        [DataMember(Order = 20)]
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 是否处理成功
        /// </summary>
        public bool IsSuccess() => ResultType != PayProcessResType.Error && string.IsNullOrEmpty(ErrorMsg);

        /// <summary>
        /// 设置错误结果
        /// </summary>
        public PayProcessResult Error(string errorMsg)
        {
            ResultType = PayProcessResType.Error;
            ErrorMsg = errorMsg;
            return this;
        }
    }
}
using System.Runtime.Serialization;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.Payment
{
    /// <summary>
    /// 支付通知的处理结果
    /// </summary>
    [DataContract]
    public class NotifyProcessResult : IPayProcessRes
    {
        [DataMember(Order = 1)]
        public PayProcessResType ResultType { get; set; }

        /// <summary>
        /// 支付渠道方的流水号
        /// </summary>
        [DataMember(Order = 2)]
        public string? GatewayTransId { get; set; }
        
        [DataMember(Order = 3)]
        public long TransId { get; set; }

        /// <summary>
        /// 附加的结果数据
        /// </summary>
        [DataMember(Order = 4)]
        public string? AttachData { get; set; }

        /// <summary>
        /// 向通知方响应的内容（Http response content）
        /// </summary>
        [DataMember(Order = 5)]
        public string? ResponseContent { get; set; }

        /// <summary>
        /// 错误描述信息
        /// </summary>
        [DataMember(Order = 20)]
        public string? ErrorMsg { get; set; }

        

    }
}
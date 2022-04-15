using System.Runtime.Serialization;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.Payment
{
    /*/// <summary>
    /// 处理订单的参数
    /// </summary>
    [DataContract]
    public class OrderProcessPara
    {
        [DataMember(Order = 1)]
        public long TransId { get; set; }

        /// <summary>
        /// 支付渠道的流水号
        /// </summary>
        [DataMember(Order = 2)]
        public string GatewayTransId { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        [DataMember(Order = 3)]
        public string? ExtraInfo { get; set; }
    }

    [DataContract]
    public class OrderProcessResult : IGeneralResObj
    {
        /// <summary>
        /// 是否重复调用了处理
        /// </summary>
        [DataMember(Order = 1)]
        public bool IsReHandle { get; set; }

        /// <summary>
        /// 错误描述信息
        /// </summary>
        [DataMember(Order = 20)]
        public string? ErrorMsg { get; set; }
    }*/
}
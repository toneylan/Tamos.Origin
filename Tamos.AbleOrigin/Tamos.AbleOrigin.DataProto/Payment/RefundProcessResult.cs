using System.Runtime.Serialization;

namespace Tamos.AbleOrigin.DataProto.Payment
{
    /// <summary>
    /// 退款处理结果
    /// </summary>
    [DataContract]
    public class RefundProcessResult : IGeneralResObj
    {
        /// <summary>
        /// 错误描述信息
        /// </summary>
        [DataMember(Order = 20)]
        public string ErrorMsg { get; set; }
    }
}
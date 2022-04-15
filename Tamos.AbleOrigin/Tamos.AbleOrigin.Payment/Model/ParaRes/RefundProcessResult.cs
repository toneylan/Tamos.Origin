using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.Payment
{
    /// <summary>
    /// 退款处理结果
    /// </summary>
    public class RefundProcessResult : IGeneralResObj
    {
        /// <summary>
        /// 错误描述信息
        /// </summary>
        public string? ErrorMsg { get; set; }
    }
}
namespace Tamos.AbleOrigin.Payment
{
    /// <summary>
    /// 退款上下文
    /// </summary>
    public class RefundContext
    {
        /// <summary>
        /// 当前退款请求流水号，用于唯一标识某次退款。
        /// </summary>
        public long TransId { get; set; }

        /// <summary>
        /// 要退款的支付流水号
        /// </summary>
        public long TargetTransId { get; set; }

        /// <summary>
        /// 退款金额 元
        /// </summary>
        public decimal RefundFee { get; set; }

        /// <summary>
        /// 操作人员
        /// </summary>
        public string? Operator { get; set; }
    }
}
namespace Tamos.AbleOrigin.Payment
{
    /// <summary>
    /// 退款上下文
    /// </summary>
    public class RefundContext
    {
        /// <summary>
        /// 待退款的支付流水号
        /// </summary>
        public string TargetTranId { get; set; }

        /// <summary>
        /// 退款金额 元
        /// </summary>
        public decimal RefundAmount { get; set; }
    }
}
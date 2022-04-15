namespace Tamos.AbleOrigin.Payment.ShouQianBa
{
    internal class NotifyPara : ISqbOrderInfo
    {
        /// <summary>
        /// [Y(16)]收钱吧系统内部唯一订单号	"7892259488292938"
        /// </summary>
        public string sn { get; set; }
                
        /// <summary>
        /// [Y(32)]商户系统订单号	"7654321132"
        /// </summary>
        public string client_sn { get; set; }
                
        /// <summary>
        /// [N(64)]支付通道交易凭证号，只有支付成功时才有值返回	"2013112011001004330000121536"
        /// </summary>
        public string? trade_no { get; set; }
                
        /// <summary>
        /// [Y(32)]本次操作产生的流水的状态	"SUCCESS"
        /// </summary>
        public string status { get; set; }
                
        /// <summary>
        /// [Y(32)]当前订单状态	"PAID"
        /// </summary>
        public string order_status { get; set; }
                
        /// <summary>
        /// [Y(2)]一级支付方式，取值见附录《支付方式列表》	"1"
        /// </summary>
        public string payway { get; set; }
                
        /// <summary>
        /// [Y(2)]二级支付方式，取值见附录《二级支付方式列表》	"1"
        /// </summary>
        public string sub_payway { get; set; }

        /// <summary>
        /// 支付方式名称（实际有，官方文档暂时没列出）
        /// </summary>
        public string? payway_name { get; set; }
                
        /// <summary>
        /// [N(64)]支付平台（微信，支付宝）上的付款人ID	"2801003920293239230239"
        /// </summary>
        public string? payer_uid { get; set; }
                
        /// <summary>
        /// [N(128)]支付平台上(微信，支付宝)的付款人账号	"134**3920"
        /// </summary>
        public string? payer_login { get; set; }
                
        /// <summary>
        /// [Y(10)]本次交易总金额	"10000"
        /// </summary>
        public string total_amount { get; set; }
                
        /// <summary>
        /// [Y(10)]如果没有退款，这个字段等于total_amount。否则等于 total_amount减去退款金额	"0"
        /// </summary>
        public string net_amount { get; set; }
                
        /// <summary>
        /// [Y(10)]本次支付金额	"10000"
        /// </summary>
        public string settlement_amount { get; set; }
                
        /// <summary>
        /// [Y(64)]本次交易概述	"Pizza"
        /// </summary>
        public string subject { get; set; }
                
        /// <summary>
        /// [Y(13)]时间戳	"1449646835244"
        /// </summary>
        public string finish_time { get; set; }
                
        /// <summary>
        /// [N(13)]时间戳，只有支付成功时才有值返回	"1449646835244"
        /// </summary>
        public string? channel_finish_time { get; set; }
                
        /// <summary>
        /// [Y(32)]门店操作员	"张三丰"
        /// </summary>
        public string @operator { get; set; }
                
        /// <summary>
        /// [N(64)]透传参数	{"tips": "200"}
        /// </summary>
        public string? reflect { get; set; }
                
        /*/// <summary>
        /// [N]格式为json，内容有两部分 goods_details为数组，内容为核销单品信息，voucher_details为数组，内容为核销券信息。	详见优惠详情介绍
        /// </summary>
        public json? provider_response { get; set; }
        
         payment_list	活动优惠	JSON	N	格式为json payment_list的值为数组，每一个元素包含两个字段，一个是type支付名称，一个amount_total支付金额	
        "payment_list": [{"type": "BANKCARD_DEBIT","amount_total": "1"},{"type": "DISCOUNT_CHANNEL_MCH","amount_total": "100"}]
         */
    }
}
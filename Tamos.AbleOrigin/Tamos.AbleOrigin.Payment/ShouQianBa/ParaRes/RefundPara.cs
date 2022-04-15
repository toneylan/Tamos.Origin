namespace Tamos.AbleOrigin.Payment.ShouQianBa
{
    internal class RefundPara
    {
        /// <summary>
        /// [Y(32)]收钱吧终端ID	"00101010029201012912"
        /// </summary>
        public string terminal_sn { get; set; }
                
        /// <summary>
        /// [N(16)]收钱吧系统内部唯一订单号	"7892259488292938"
        /// </summary>
        public string? sn { get; set; }
                
        /// <summary>
        /// [N(32)]商户系统订单号。sn与client_sn不能同时为空，优先按照sn查找订单，如果没有，再按照client_sn查询
        /// </summary>
        public string? client_sn { get; set; }
                
        /// <summary>
        /// [Y(20)]商户退款所需序列号，用于唯一标识某次退款请求，以防止意外的重复退款。正常情况下，对同一笔订单进行多次退款请求时该字段不能重复；<br/>
        /// 而当通信质量不佳，终端不确认退款请求是否成功，自动或手动发起的退款请求重试，则务必要保持序列号不变
        /// </summary>
        public string refund_request_no { get; set; }
                
        /// <summary>
        /// [Y(64)]执行本次退款的操作员	"Obama"
        /// </summary>
        public string @operator { get; set; }
                
        /// <summary>
        /// [Y(10)]退款金额	"100"
        /// </summary>
        public string refund_amount { get; set; }
                
        /// <summary>
        /// [N(64)]任何调用者希望原样返回的信息，可以用于关联商户ERP系统的订单或记录附加订单内容	{ "tips": "200" }
        /// </summary>
        public string? reflect { get; set; }
        
        /*/// <summary>
        /// [N]收钱吧与特定第三方单独约定的参数集合,json格式，最多支持24个字段，每个字段key长度不超过64字节，value长度不超过256字节	{ "goods_tag": "beijing"}
        /// </summary>
        public map? 扩展参数集合 { get; set; }
                
        /// <summary>
        /// [N]格式为json goods_details的值为数组，每一个元素包含五个字段，一个是goods_id商品的编号，一个是goods_name商品名称，一个是quantity商品数量，一个是price商品单价，单位为分，一个是promotion_type优惠类型，0:没有优惠 1: 支付机构优惠，为1会把相关信息送到支付机构	"goods_details": [{"goods_id": "wx001","goods_name": "苹果笔记本电脑","quantity": 1,"price": 2,"promotion_type": 0},{"goods_id": "wx002","goods_name":"tesla","quantity": 1,"price": 2,"promotion_type": 1}]
        /// </summary>
        public json? goods_details { get; set; }*/
        
    }

    internal class RefundRes : ISqbOrderInfo
    {
        /// <summary>
        /// [Y(32)]收钱吧终端ID，可使用英文字母和数字	"103939292020"
        /// </summary>
        public string terminal_sn { get; set; }
                
        /// <summary>
        /// [Y(16)]收钱吧系统内部唯一订单号	"7894259244061958"
        /// </summary>
        public string sn { get; set; }
                
        /// <summary>
        /// [Y(64)]商户系统订单号。	"22345677767776"
        /// </summary>
        public string client_sn { get; set; }
                
        /// <summary>
        /// [Y(32)]本次退款对应的流水的状态	"SUCCESS"
        /// </summary>
        public string status { get; set; }
                
        /// <summary>
        /// [Y(32)]当前订单状态	"REFUNDED"
        /// </summary>
        public string order_status { get; set; }
                
        /// <summary>
        /// [Y(32)]订单支付方式	"3"
        /// </summary>
        public string payway { get; set; }
                
        /// <summary>
        /// [Y(128)]"微信"
        /// </summary>
        public string payway_name { get; set; }
                
        /// <summary>
        /// [Y(64)]支付宝或微信的订单号	"2006101016201512090096528672"
        /// </summary>
        public string trade_no { get; set; }
                
        /// <summary>
        /// [Y(10)]原始交易实收金额	"100"
        /// </summary>
        public string total_amount { get; set; }
                
        /// <summary>
        /// [Y(10)]实收金额减退款金额	"0"
        /// </summary>
        public string net_amount { get; set; }
                
        /// <summary>
        /// [Y(10)]本次操作退款金额	"100"
        /// </summary>
        public string settlement_amount { get; set; }
                
        /// <summary>
        /// [N(13)]时间戳，本次退款动作在收钱吧的完成时间。退款成功有值返回。	"1449646835244"
        /// </summary>
        public string? finish_time { get; set; }
                
        /// <summary>
        /// [N(13)]时间戳，本次退款动作在微信或支付宝的完成时间。退款成功有值返回。	"1449646835221"
        /// </summary>
        public string? channel_finish_time { get; set; }
                
        /// <summary>
        /// [Y(32)]交易时候的商品概述	"wx"
        /// </summary>
        public string subject { get; set; }
                
        /// <summary>
        /// [Y(32)]执行本次退款的操作员	"Obama"
        /// </summary>
        public string @operator { get; set; }

    }
}
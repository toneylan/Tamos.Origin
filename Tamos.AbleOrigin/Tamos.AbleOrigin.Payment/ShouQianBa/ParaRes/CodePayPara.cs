namespace Tamos.AbleOrigin.Payment.ShouQianBa
{
    internal class CodePayPara
    {
        /// <summary>
        /// [Y(32)]收钱吧终端ID，不超过32位的纯数字	"00101010029201012912"
        /// </summary>
        public string terminal_sn { get; set; }
                
        /// <summary>
        /// [Y(32)]必须在商户系统内唯一；且长度不超过32字节	"18348290098298292838"
        /// </summary>
        public string client_sn { get; set; }
                
        /// <summary>
        /// [Y(10)]以分为单位,不超过10位纯数字字符串,超过1亿元的收款请使用银行转账	"1000"
        /// </summary>
        public string total_amount { get; set; }
                
        /// <summary>
        /// [N]非必传。内容为数字的字符串。一旦设置，则根据支付码判断支付通道的逻辑失效	1:支付宝
        /// </summary>
        public string? payway { get; set; }
                
        /// <summary>
        /// [Y(32)]不超过32字节	"130818341921441147"
        /// </summary>
        public string dynamic_id { get; set; }
                
        /// <summary>
        /// [Y(64)]本次交易的简要介绍	"Pizza"
        /// </summary>
        public string subject { get; set; }
                
        /// <summary>
        /// [Y(32)]发起本次交易的操作员	Obama
        /// </summary>
        public string @operator { get; set; }
                
        /// <summary>
        /// [N(256)]对商品或本次交易的描述	
        /// </summary>
        public string? description { get; set; }
                
        /// <summary>
        /// [N]经纬度必须同时出现	"121.615459404"
        /// </summary>
        public string? longitude { get; set; }
                
        /// <summary>
        /// [N]经纬度必须同时出现	"31.4056441552"
        /// </summary>
        public string? latitude { get; set; }
                
        /// <summary>
        /// [N]
        /// </summary>
        public string? device_id { get; set; }
                
        /*/// <summary>
        /// [N]收钱吧与特定第三方单独约定的参数集合,json格式，最多支持24个字段，每个字段key长度不超过64字节，value长度不超过256字节	{ "goods_tag": "beijing"}
        /// </summary>
        public map? 扩展参数集合 { get; set; }
                
        /// <summary>
        /// [N]格式为json goods_details的值为数组，每一个元素包含五个字段，goods_id商品的编号，goods_name商品名称，quantity商品数量，price商品单价，单位为分，promotion_type优惠类型，0:没有优惠 1: 支付机构优惠，为1会把相关信息送到支付机构	"goods_details": [{"goods_id": "wx001","goods_name": "苹果笔记本电脑","quantity": 1,"price": 2,"promotion_type": 0},{"goods_id":"wx002","goods_name":"tesla","quantity": 1,"price": 2,"promotion_type": 1}]
        /// </summary>
        public json? goods_details { get; set; }*/
                
        /// <summary>
        /// [N(64)]任何调用者希望原样返回的信息，可以用于关联商户ERP系统的订单或记录附加订单内容	{ "tips": "200" }
        /// </summary>
        public string? reflect { get; set; }
                
        /// <summary>
        /// [N(128)]支付回调的地址	例如：www.baidu.com 如果支付成功通知时间间隔为1s,5s,30s,600s
        /// </summary>
        public string? notify_url { get; set; }
    }

    internal class CodePayRes : ISqbOrderInfo
    {
        #region Prop

        /// <summary>
        /// [Y(32)]收钱吧终端ID	"01939202039923029"
        /// </summary>
        public string terminal_sn { get; set; }
                
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
        /// [Y(128)]支付宝
        /// </summary>
        public string? payway_name { get; set; }
                
        /// <summary>
        /// [Y(2)]二级支付方式，取值见附录《二级支付方式列表》	"1"
        /// </summary>
        public string sub_payway { get; set; }
                
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
        /// [N(13)]时间戳，只有order_status为最终状态时才会返回	"1449646835244"
        /// </summary>
        public string? finish_time { get; set; }
                
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
        public json? provider_response { get; set; }*/
                
        /*/// <summary>
        /// [N]格式为json payment_list的值为数组，每一个元素包含两个字段，一个是type支付名称，一个amount_total支付金额	"payment_list": [{"type": "BANKCARD_DEBIT","amount_total": "1"},{"type": "DISCOUNT_CHANNEL_MCH","amount_total": "100"}]
        /// </summary>
        public json? payment_list { get; set; }*/

        #endregion
    }
}
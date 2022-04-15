namespace Tamos.AbleOrigin.Payment.ShouQianBa
{
    internal class QueryPara
    {
        /// <summary>
        /// [Y(32)]收钱吧终端ID
        /// </summary>
        public string terminal_sn { get; set; }
                
        /// <summary>
        /// [N(16)]收钱吧系统唯一订单号
        /// </summary>
        public string? sn { get; set; }
                
        /// <summary>
        /// [N(32)]商户自己订号
        /// </summary>
        public string? client_sn { get; set; }
                
        /// <summary>
        /// [N(20)]调用退款接口时，传入得值，可用于多次部分退款场景下，查询某次部分退款的结果
        /// </summary>
        public string? refund_request_no { get; set; }
    }

    internal interface ISqbOrderInfo
    {
        /// <summary>
        /// 收钱吧系统内部唯一订单号
        /// </summary>
        string sn { get; set; }

        /// <summary>
        /// 当前订单状态，指示交易是否已成功支付或退款
        /// </summary>
        string order_status { get; set; }

        /// <summary>
        /// 支付方式名称
        /// </summary>
        string? payway_name { get; set; }

        /// <summary>
        /// 支付宝或微信的订单号
        /// </summary>
        string? trade_no { get; set; }
    }

    internal class QueryRes : ISqbOrderInfo
    {
        /// <summary>
        /// [Y(32)]收钱吧终端ID
        /// </summary>
        public string terminal_sn { get; set; }
                
        /// <summary>
        /// [Y(16)]收钱吧系统内部唯一订单号
        /// </summary>
        public string sn { get; set; }
                
        /// <summary>
        /// [Y(32)]商户系统订单号。
        /// </summary>
        public string client_sn { get; set; }
                
        /// <summary>
        /// [Y(32)]本次操作对应的流水的状态
        /// </summary>
        public string status { get; set; }
                
        /// <summary>
        /// [Y(32)]当前订单状态，指示交易是否已成功支付或退款<br/>
        /// 判断当前收钱吧订单的交易状态. 非常重要！
        /// </summary>
        public string order_status { get; set; }
                
        /// <summary>
        /// [Y(32)]订单支付方式
        /// </summary>
        public string payway { get; set; }
                
        /// <summary>
        /// [Y(128)]支付方式名称
        /// </summary>
        public string? payway_name { get; set; }
                
        /// <summary>
        /// [Y(64)]订单付款人的id	如微信"oGFfksxxsiXIWSPsNy4Mu-YhBB-I"
        /// </summary>
        public string payer_uid { get; set; }
                
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
        /// [N(10)]订单支付时等于本次支付金额，订单退款时等于本次操作退款金额	"100"
        /// </summary>
        public string? settlement_amount { get; set; }
                
        /// <summary>
        /// [N(13)]时间戳，本次动作在收钱吧的完成时间，只有支付成功、退款成功、撤单成功才有值返回	"1449646835244"
        /// </summary>
        public string? finish_time { get; set; }
                
        /// <summary>
        /// [N(13)]时间戳，本次动作在微信或支付宝的完成时间，只有支付成功、退款成功、撤单成功才有值返回	"1449646835221"
        /// </summary>
        public string? channel_finish_time { get; set; }
                
        /// <summary>
        /// [Y(32)]交易时候的商品概述	"wx"
        /// </summary>
        public string subject { get; set; }
                
        /// <summary>
        /// [Y(64)]执行上次业务动作的操作员	"Obama"
        /// </summary>
        public string @operator { get; set; }
                
        /*/// <summary>
        /// [N]格式为json，内容有两部分 goods_details为数组，内容为核销单品信息，voucher_details为数组，内容为核销券信息。	详见优惠详情介绍
        /// </summary>
        public json provider_response { get; set; }
                
        /// <summary>
        /// [N]格式为json payment_list的值为数组，每一个元素包含两个字段，一个是type支付名称，一个amount_total支付金额
        /// </summary>
        public json payment_list { get; set; }*/
    }
}
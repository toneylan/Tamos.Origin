using System;
using System.Collections.Generic;
using Tamos.AbleOrigin.Common;

namespace Tamos.AbleOrigin.Payment
{
    /// <summary>
    /// 支付的上下文信息
    /// </summary>
    public class PaymentContext
    {
        /// <summary>
        /// 支付流水号，通常对应支付平台的“out_trade_no”参数
        /// </summary>
        public string TransactionId { get; set; }
        
        /// <summary>
        /// 需要支付的总金额
        /// </summary>
        public decimal TotalFee { get; set; }

        /// <summary>
        /// 支付超时时间
        /// </summary>
        public DateTime PayExpireTime { get; set; }

        /// <summary>
        /// 付款码，如微信、支付宝的支付授权码
        /// </summary>
        public string PayCode { get; set; }

        /// <summary>
        /// 支付信息标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 支付信息描述
        /// </summary>
        public string Description { get; set; }

        #region Para store

        private Dictionary<string, string> _paras;

        /// <summary>
        /// get/set 其他支付参数
        /// </summary>
        public string this[string key]
        {
            get => (_paras ??= new Dictionary<string, string>()).GetValue(key);
            set => (_paras ??= new Dictionary<string, string>()).SetValue(key, value);
        }

        #endregion
    }

    public static class PayContextPara
    {
        public const string UserId = "UserId";
        //public const string TradeType = "TradeType";
        public const string ProductId = "ProductId";
        public const string SubOpenId = "SubOpenId";

        //public const string DeviceInfo = "DeviceInfo";

    }
}
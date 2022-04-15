using System;
using System.Collections.Generic;

namespace Tamos.AbleOrigin.Payment
{
    /// <summary>
    /// 支付的上下文信息
    /// </summary>
    public class PaymentContext
    {
        /// <summary>
        /// 支付流水号，对应支付平台的“商户订单号”
        /// </summary>
        public long TransId { get; set; }
        
        /// <summary>
        /// 需要支付的总金额
        /// </summary>
        public decimal TotalFee { get; set; }

        /// <summary>
        /// 支付超时-秒
        /// </summary>
        public int TimeoutSeconds { get; set; }

        /// <summary>
        /// 微信、支付宝等付款码
        /// </summary>
        public string? PayCode { get; set; }

        /// <summary>
        /// 支付信息标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 支付信息描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 操作人员
        /// </summary>
        public string? Operator { get; set; }

        /// <summary>
        /// 支付后回跳的Url，适用于跳转到支付网关的场景。
        /// </summary>
        public string? ReturnUrl { get; set; }

        #region Para store

        private Dictionary<string, string>? _paras;

        /// <summary>
        /// get/set 其他支付参数
        /// </summary>
        public string? this[string key]
        {
            get => (_paras ??= new Dictionary<string, string>()).GetValueOrDefault(key);
            set => (_paras ??= new Dictionary<string, string>()).SetValue(key, value);
        }

        #endregion
    }

    /*public static class PayContextPara
    {
        public const string UserId = "UserId";
        //public const string TradeType = "TradeType";
        public const string ProductId = "ProductId";
        public const string SubOpenId = "SubOpenId";

        //public const string DeviceInfo = "DeviceInfo";

    }*/
}
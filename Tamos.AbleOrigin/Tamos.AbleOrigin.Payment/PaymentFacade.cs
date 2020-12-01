using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Tamos.AbleOrigin.Common;
using Tamos.AbleOrigin.DataProto.Payment;
using Tamos.AbleOrigin.Payment.WxPay;

namespace Tamos.AbleOrigin.Payment
{
    /// <summary>
    /// 支付流程类Facade
    /// </summary>
    public class PaymentFacade
    {
        #region Static member

        private static Dictionary<PayGatewayTypes, Func<CommonPayConfig, PaymentGatewayBase>> _gatewayBuilders;

        /// <summary>
        /// 让具体应用可扩展实现支付渠道。
        /// </summary>
        public static Dictionary<PayGatewayTypes, Func<CommonPayConfig, PaymentGatewayBase>> GatewayBuilders =>
            _gatewayBuilders ??= new Dictionary<PayGatewayTypes, Func<CommonPayConfig, PaymentGatewayBase>>();

        /// <summary>
        /// 通过支付渠道及配置信息，创建PaymentFacade。
        /// </summary>
        public static PaymentFacade Create(PayGatewayTypes gatewayType, CommonPayConfig payConfig)
        {
            return new PaymentFacade(gatewayType, payConfig);
        }

        /// <summary>
        /// 导出Http请求信息并保存为文件
        /// </summary>
        public static void DumpRequestToFile(HttpRequest request, string dirName)
        {
            PaymentGatewayBase.DumpRequestToFile(request, dirName);
        }

        #endregion

        #region Ctor

        private readonly PaymentGatewayBase _paymentGateway;

        private PaymentFacade(PayGatewayTypes gatewayType, CommonPayConfig payConfig)
        {
            if (payConfig == null) throw new ArgumentNullException(nameof(payConfig), "未提供支付平台配置");

            _paymentGateway = gatewayType switch
            {
                PayGatewayTypes.WxJsApiPay => new WxJsApiPayGateway(payConfig),
                _ => _gatewayBuilders?.GetValue(gatewayType)?.Invoke(payConfig)
            };

            if (_paymentGateway == null) throw new ArgumentOutOfRangeException("gatewayType", "未知的支付方式");
            _paymentGateway.GatewayType = gatewayType;
        }

        #endregion

        #region Payment handle

        /// <summary>
        /// 处理支付请求
        /// </summary>
        public PayProcessResult PayRequest(PaymentContext paymentContext)
        {
            return _paymentGateway.ProcessPayRequest(paymentContext);
        }
        
        /// <summary>
        /// 处理支付结果通知
        /// </summary>
        public NotifyProcessResult PayNotify(HttpContext httpContext, IOrderProcessor orderProcessor)
        {
            return _paymentGateway.ProcessPayNotify(httpContext, orderProcessor);
        }

        /// <summary>
        /// 支付退款
        /// </summary>
        public RefundProcessResult Refund(RefundContext context)
        {
            return _paymentGateway.ProcessRefund(context);
        }

        /// <summary>
        /// 查询支付结果
        /// </summary>
        public PayProcessResult QueryPaymentResult(long transactionId, IOrderProcessor orderProcessor)
        {
            return _paymentGateway.QueryPaymentResult(transactionId, orderProcessor);
        }

        #endregion
    }
}
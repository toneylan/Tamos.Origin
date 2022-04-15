using System;
using System.Threading.Tasks;
using Tamos.AbleOrigin.DataProto;
using Tamos.AbleOrigin.Payment.ShouQianBa;

namespace Tamos.AbleOrigin.Payment
{
    public abstract class PaymentGateway
    {
        protected CommonPayProfile PayProfile { get; }

        protected PaymentGateway(CommonPayProfile payProfile)
        {
            PayProfile = payProfile;
        }

        /// <summary>
        /// 处理支付请求
        /// </summary>
        public abstract Task<PayProcessResult> ProcessPayRequest(PaymentContext context);
        
        /// <summary>
        /// 处理支付平台的结果通知
        /// </summary>
        public abstract ValueTask<NotifyProcessResult> ProcessPayNotify(PayNotifyContext context);

        /// <summary>
        /// 处理支付退款
        /// </summary>
        public abstract Task<RefundProcessResult> ProcessRefund(RefundContext context);

        /// <summary>
        /// 查询支付结果
        /// </summary>
        public virtual Task<PayQueryResult> QueryPayResult(long transId)
        {
            return Task.FromResult(new PayQueryResult().Error("未实现订单查询"));
        }

        #region Static creater

        /*private static Dictionary<PayGatewayType, Func<CommonPayProfile, PaymentGateway>>? _gatewayBuilders;

        /// <summary>
        /// 让具体应用可扩展实现支付渠道。
        /// </summary>
        public static Dictionary<PayGatewayType, Func<CommonPayProfile, PaymentGateway>> GatewayBuilders =>
            _gatewayBuilders ??= new Dictionary<PayGatewayType, Func<CommonPayProfile, PaymentGateway>>();*/

        /// <summary>
        /// 通过支付渠道及配置信息，创建PaymentGateway。
        /// </summary>
        public static PaymentGateway Create(CommonPayProfile payProfile)
        {
            switch (payProfile.GatewayType)
            {
                case PayGatewayType.SqbWap:
                case PayGatewayType.SqbPayCode:
                    return new ShouQianBaGateway(payProfile);
                default:
                    throw new NotImplementedException("未实现支付渠道：" + payProfile.GatewayType);
            }

            //if (_paymentGateway == null) throw new ArgumentOutOfRangeException("gatewayType", "未知的支付方式");
        }

        #endregion
    }
}

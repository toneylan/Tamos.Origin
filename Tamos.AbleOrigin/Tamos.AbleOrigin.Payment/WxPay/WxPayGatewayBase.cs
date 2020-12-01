using System;
using Microsoft.AspNetCore.Http;
using Tamos.AbleOrigin.Common;
using Tamos.AbleOrigin.DataProto;
using Tamos.AbleOrigin.DataProto.Payment;
using Tamos.AbleOrigin.Log;

namespace Tamos.AbleOrigin.Payment.WxPay
{
    internal abstract class WxPayGatewayBase : PaymentGatewayBase
    {
        protected const string Trade_JSAPI = "JSAPI";
        protected const string Trade_NATIVE = "NATIVE";

        private WxPayConfig _wxPayConfig;
        protected WxPayConfig WxPayConfig => _wxPayConfig ??= new WxPayConfig(PayConfig);

        protected WxPayGatewayBase(CommonPayConfig config) : base(config)
        {
            
        }

        #region PayData

        /// <summary>
        /// 检查并设置统一下单参数
        /// </summary>
        internal string CheckAndSetRequestPara(PaymentContext context, string tradeType, out WxPayData data)
        {
            data = null;
            if (string.IsNullOrEmpty(context?.TransactionId) || string.IsNullOrEmpty(context.Title)) return "支付信息为空";
            
            //交易类型：JSAPI，NATIVE，APP等
            if (tradeType == Trade_JSAPI)
            {
                if (context[PayContextPara.UserId] == null && context[PayContextPara.SubOpenId] == null) return "缺少微信用户OpenId参数";
            }
            else if (tradeType == Trade_NATIVE)
            {
                if (context[PayContextPara.ProductId] == null) return "缺少商品Id参数";
            }

            //支付参数设置
            data = new WxPayData(WxPayConfig);
            data.SetValue("out_trade_no", context.TransactionId); //商户系统订单号，同一个商户号下唯一。
            data.SetValue("total_fee", (int)(context.TotalFee * 100)); //总金额: 分
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易起始时间
            data.SetValue("time_expire", context.PayExpireTime.ToString("yyyyMMddHHmmss"));//结束时间，失效时间间隔必须大于5分钟
            data.SetValue("trade_type", tradeType);
            data.SetValue("body", context.Title); //商品描述
            //data.SetValue("attach", "Pay="); //附加数据，在查询API和支付通知中原样返回，可作为自定义参数使用。
            data.Set("product_id", context[PayContextPara.ProductId]); //trade_type=NATIVE时（即扫码支付），此参数必传
            data.Set("openid", context[PayContextPara.UserId]); //trade_type=JSAPI时（即公众号支付），此参数必传
            data.Set("sub_openid", context[PayContextPara.SubOpenId]); //用户在子商户下的唯一标识。openid和sub_openid可以选传其中之一，如果选择传sub_openid,则必须传sub_appid

            //服务商模式，设置子商户
            if (!string.IsNullOrEmpty(PayConfig.SubMerAccount))
            {
                data.SetValue("sub_mch_id", PayConfig.SubMerAccount);
                data.Set("sub_appid", PayConfig.SubAppId); //如果传了sub_openid,则必须传sub_appid
            }

            return null;
        }

        /// <summary>
        /// 判断调用结果是否成功
        /// </summary>
        internal bool IsResultSuccess(WxPayData result)
        {
            return result.GetValueStr("return_code") == "SUCCESS" && result.GetValueStr("result_code") == "SUCCESS";
        }

        #endregion

        #region Handle Notify
        
        public override NotifyProcessResult ProcessPayNotify(HttpContext httpContext, IOrderProcessor orderProcessor)
        {
            return HandlePayNotify(httpContext, orderProcessor);
        }

        /// <summary>
        /// 处理支付结果通知
        /// </summary>
        protected NotifyProcessResult HandlePayNotify(HttpContext httpContext, IOrderProcessor orderProcessor)
        {
            //通知参数解析
            var notifyData = Notify.GetNotifyData(httpContext.Request, WxPayConfig, out var error);
            var handRes = new NotifyProcessResult
            {
                ErrorMsg = error,
                TransactionId = notifyData.GetValueStr("out_trade_no"),
                GatewayTranId = notifyData.GetValueStr("transaction_id")
            };
            if (error.NotNull()) return handRes;

            //回复结果
            var replyRes = new WxPayData(WxPayConfig);
            Action<string> FailRes = msg =>
            {
                replyRes.SetValue("return_code", "FAIL");
                replyRes.SetValue("return_msg", msg);
                handRes.ErrorMsg = msg;
                //handRes.ResultType = PayProcessResType.Error;
            };
            
            //检查支付结果中transaction_id是否存在
            if (handRes.GatewayTranId.NotNull() && notifyData.GetValueStr("result_code") == "SUCCESS")
            {
                //--发起订单查询，判断订单真实性
                if (QueryOrderSuccess(handRes.GatewayTranId))
                {
                    //订单处理逻辑
                    var orderHdRes = orderProcessor.OnOrderPaid(new OrderProcessPara
                    {
                        TransactionId = handRes.TransactionId.ToLong(),
                        GatewayTranId = handRes.GatewayTranId
                    });
                    if (orderHdRes.IsSuccess())
                    {
                        replyRes.SetValue("return_code", "SUCCESS");
                        replyRes.SetValue("return_msg", "OK");
                        httpContext.Response.WriteAsync(replyRes.ToXml());
                        //httpContext.Response.End();
                        
                        LogService.DebugFormat("Wxpay process notify success:{0}", replyRes.ToXml());
                        //handRes.ResultType = PayProcessResType.PaymentSuccess;
                        return handRes; //---成功直接返回
                    }
                    
                    //订单处理失败返回非success，网关将持续通知
                    FailRes("订单支付处理失败: " + orderHdRes.ErrorMsg);
                }
                else //订单查询失败
                {
                    FailRes("订单查询失败");
                }
            }
            else
            {
                //若transaction_id不存在，则立即返回结果给微信支付后台
                FailRes("支付结果中微信订单号不存在");
            }

            //返回失败结果给微信支付后台
            httpContext.Response.WriteAsync(replyRes.ToXml());
            //httpContext.Response.End();

            LogService.ErrorFormat("Wxpay process notify failure:{0}", replyRes.ToXml());
            return handRes;
        }

        /// <summary>
        /// 使用微信流水号查询订单是否成功
        /// </summary>
        private bool QueryOrderSuccess(string wxTransactionId, string merTranId = null)
        {
            var req = new WxPayData(WxPayConfig);
            if (wxTransactionId.NotNull()) req.SetValue("transaction_id", wxTransactionId);
            else req.SetValue("out_trade_no", merTranId);

            var res = WxPayApi.OrderQuery(req);
            if (IsResultSuccess(res) && res.GetValueStr("trade_state") == "SUCCESS")
            {
                return true;
            }
            return false;
        }

        #endregion
        
        #region 查询订单
        
        public override PayProcessResult QueryPaymentResult(long transactionId, IOrderProcessor orderProcessor)
        {
            var res = new PayProcessResult();

            //--发起订单查询，判断订单真实性
            if (!QueryOrderSuccess(null, transactionId.ToString()))
            {
                res.ResultType = PayProcessResType.RequestSuccess;
                return res;
            }

            //支付成功，调用订单处理逻辑
            res.ResultType = PayProcessResType.PaymentSuccess;
            if (orderProcessor == null) return res;

            var orderHdRes = orderProcessor.OnOrderPaid(new OrderProcessPara
            {
                TransactionId = transactionId,
                GatewayTranId = null
            });

            LogService.InfoFormat("Wxpay rehandle pay:{0}, is success:{1}", transactionId, orderHdRes.IsSuccess());

            //记录处理失败消息
            res.ErrorMsg = orderHdRes.ErrorMsg;

            return res;
        }

        #endregion
    }
}
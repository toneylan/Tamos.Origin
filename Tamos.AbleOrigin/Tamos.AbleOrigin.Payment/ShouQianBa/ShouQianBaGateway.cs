using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.Payment.ShouQianBa
{
    public class ShouQianBaGateway : PaymentGateway
    {
        /*vendor_sn(开发者序列号)：91801418
        vendor_key(开发者密钥)：ef7ab44139dcf456aa48944918df0dd8

        app_id(应用ID)：目前仅用于终端激活
            公众号支付 ：2021092900004243
            b扫c ： 2021092900004242
        */

        private const string api_domain = "https://vsi-api.shouqianba.com";
        private const int CodePayTimeout = 45; //B扫C超时（秒）

        public ShouQianBaGateway(CommonPayProfile payProfile) : base(payProfile)
        {
        }

        #region PayRequest

        public override Task<PayProcessResult> ProcessPayRequest(PaymentContext context)
        {
            return PayProfile.GatewayType == PayGatewayType.SqbWap ? Task.FromResult(WapPay(context)) : CodePay(context);
        }

        private PayProcessResult WapPay(PaymentContext context)
        {
            //订单有效支付时间为4分钟;
            var para = new GeneralParaBag()
                .Set("terminal_sn", GetTerminalSn()) //收钱吧终端ID
                .Set("client_sn", context.TransId.ToString()) //商户系统订单号, 必须在商户系统内唯一
                .Set("total_amount", (context.TotalFee * 100).ToString("0")) //以分为单位,不超过10位纯数字字符串
                .Set("subject", context.Title) //本次交易的概述
                .Set("operator", context.Operator ?? "Customer") //发起本次交易的操作员
                .Set("notify_url", PayProfile.NotifyUrl) //支付回调的地址
                .Set("return_url", context.ReturnUrl); //处理完请求后，当前页面自动跳转到商户网站里指定页面的http路径
            /*
            .Set("payway", )   //支付方式，不传默认选择当前环境支持的支付方式。如在支付宝客户端打开则使用支付宝支付。
            .Set("longitude", )   //经纬度必须同时出现
            .Set("latitude", )   //经纬度必须同时出现
            .Set("extended", )   //收钱吧与特定第三方单独约定的参数集合,json格式，最多支持24个字段，每个字段key长度不超过64字节，value长度不超过256字节
            .Set("reflect", )   //任何调用者希望原样返回的信息*/
            if (context.Description.NotNull()) para.Set("description", context.Description); //对商品或本次交易的描述 

            //用参数原值做签名
            var sign = Encryptor.MD5Encrypt($"{para.ToUrlPara(false)}&key={GetTerminalKey()}").ToUpperInvariant();

            return new PayProcessResult
            {
                ResultType = PayProcessResType.RequestSuccess,
                ResultValue = $"https://qr.shouqianba.com/gateway?{para.ToUrlPara()}&sign={sign}",
                ResHandleWay = PayResHandleWay.Redirect
            };
        }

        private async Task<PayProcessResult> CodePay(PaymentContext context)
        {
            #region Call api

            var res = new PayProcessResult();
            if (context.PayCode.IsNull()) return res.ErrorRes("缺少付款码参数");

            var para = new CodePayPara
            {
                terminal_sn = GetTerminalSn(),
                client_sn = context.TransId.ToString(),
                total_amount = (context.TotalFee * 100).ToString("0"),
                dynamic_id = context.PayCode,
                subject = context.Title,
                @operator = context.Operator ?? "NotSet",
                description = context.Description,
                notify_url = PayProfile.NotifyUrl
            };

            var apiRes = await Call<BizData<CodePayRes>>("/upay/v2/pay", para);

            //-- 结果检查, 确定的失败或成功，直接返回
            var payState = SetProcessRes(res, apiRes);
            if (payState != PaymentState.WaitPaying) return res;

            #endregion

            //-- 轮询支付结果，间隔3秒
            const int DelaySec = 3;
            var loopCount = CodePayTimeout / DelaySec;
            while (loopCount-- > 0)
            {
                await Task.Delay(DelaySec * 1000);
                
                var qryRes = await QueryResult(context.TransId);
                payState = SetProcessRes(res, qryRes);
                
                if (payState != PaymentState.WaitPaying) return res;
            }

            //-- 超时无最终结果，不应出现才对。若有，可考虑加入撤单操作。
            return res.ErrorRes("支付超时无结果，可进行手动查单操作。");
        }
        
        #endregion

        #region PayNotify

        public override async ValueTask<NotifyProcessResult> ProcessPayNotify(PayNotifyContext context)
        {
            var res = new NotifyProcessResult();
            var auth = context.Headers?.GetValueOrDefault(HeaderAuthKey);
            if (context.Body.IsNull() || auth.IsNull()) return res.ErrorRes("缺少回调数据");
            var para = SerializeUtil.FromJson<NotifyPara>(context.Body);

            //检查Notify信息
            res.TransId = para.client_sn.ToLong();
            res.GatewayTransId = para.sn;
            var payState = para.GetPayState();
            if (payState != PaymentState.PaymentSuccess || res.TransId <= 0) return res.ErrorRes("未支付成功的通知");

            //目前没有验签，则直接查订单确定状态安全性
            var qryRes = await QueryResult(res.TransId);
            SetProcessRes(res, qryRes);
            if (res.ResultType != PayProcessResType.PaymentSuccess) return res.ErrorRes("未支付成功的通知");

            //确认支付成功
            res.ResponseContent = "success";
            return res;
        }

        #endregion

        #region Refund

        /// <summary>
        /// 退款期限3个月
        /// </summary>
        public override async Task<RefundProcessResult> ProcessRefund(RefundContext context)
        {
            var para = new RefundPara
            {
                terminal_sn = GetTerminalSn(),
                refund_request_no = context.TransId.ToString(),
                client_sn = context.TargetTransId.ToString(),
                @operator = context.Operator ?? "NotSet",
                refund_amount = (context.RefundFee * 100).ToString("0")
            };

            var apiRes = await Call<BizData<RefundRes>>("/upay/v2/refund", para);

            var res = new RefundProcessResult();
            var order = apiRes.biz_response?.data;
            if (!apiRes.IsReqSuccess || order == null) return res.Error(apiRes.GetErrorDes());

            if (order.order_status is "REFUNDED" or "PARTIAL_REFUNDED") return res;

            return res.Error($"退款失败：{apiRes.GetErrorDes()}，order_status:{order.order_status}");
        }

        #endregion

        #region QueryPayResult

        /*public override PayQueryResult QueryPayResult(long transId)
        {
            
        }*/

        private async Task<Result<BizData<QueryRes>>> QueryResult(long transId)
        {
            var para = new QueryPara
            {
                terminal_sn = GetTerminalSn(),
                client_sn = transId.ToString()
            };

            var res = await Call<BizData<QueryRes>>("/upay/v2/query", para);

            return res;
        }
        
        //解析结果
        private static PaymentState SetProcessRes<T>(IPayProcessRes res, Result<BizData<T>> apiRes) where T : class, ISqbOrderInfo
        {
            //-- 调用失败
            var order = apiRes.biz_response?.data;
            if (!apiRes.IsReqSuccess || order == null)
            {
                res.ErrorRes(apiRes.GetErrorDes());
                return PaymentState.PayFail;
            }

            // 设置结果信息
            if (res.GatewayTransId.IsNull()) res.GatewayTransId = order.sn;

            //-- 检查支付状态
            var payState = order.GetPayState();
            if (payState == PaymentState.PayFail)
            {
                //支付失败
                res.ErrorRes(apiRes.GetErrorDes());
                res.AttachData = $"order_status:{order.order_status}";
            }
            else if (payState == PaymentState.PaymentSuccess)
            {
                //已支付成功
                res.ResultType = PayProcessResType.PaymentSuccess;
                res.AttachData = $"order_status:{order.order_status}; payway_name:{order.payway_name}; trade_no:{order.trade_no}";
            }

            return payState;
        }

        #endregion

        #region Activate & Checkin

        public async Task<TerminalInfo?> Activate(ActivatePara para)
        {
            var res = await Call<TerminalInfo>("/terminal/activate", para, true);

            return res.biz_response;
        }

        public async Task<TerminalInfo?> Checkin(string deviceId)
        {
            var para = new
            {
                terminal_sn = GetTerminalSn(),
                device_id = deviceId
            };

            var res = await Call<TerminalInfo>("/terminal/checkin", para);

            return res.biz_response;
        }

        #endregion

        #region Util

        private const string HeaderAuthKey = "Authorization";

        private string GetTerminalSn()
        {
            return PayProfile.SubAppId!;
        }

        private string? GetTerminalKey()
        {
            return PayProfile.SubMerchantId;
        }

        private async Task<Result<T>> Call<T>(string relPath, object paraObj, bool isActivate = false)
        {
            try
            {
                var paraData = SerializeUtil.ToJson(paraObj);
                var sign = Encryptor.MD5Encrypt(paraData + (isActivate ? PayProfile.SecretKey : GetTerminalKey()));

                var resStr = await HttpUtil.PostAsync(api_domain + relPath, paraData, new HttpReqOptions
                {
                    ContentType = "application/json",
                    Headers = new[] { (HeaderAuthKey, $"{(isActivate ? PayProfile.AppId : GetTerminalSn())} {sign}") }
                });

                var res = SerializeUtil.FromJson<Result<T>>(resStr);
                if (!res.IsReqSuccess) LogService.ErrorFormat("收钱吧 Api:{0}, Para:{1}\nFail:{2}", relPath, paraData, resStr);
                else LogService.DebugFormat("收钱吧 Api:{0}, Para:{1}\nRes:{2}", relPath, paraData, resStr);

                //Console.WriteLine(resStr);
                return res;
            }
            catch (Exception e)
            {
                LogService.Error(e);
                return Result<T>.Error(e.Message);
            }
        }

        #endregion
    }
}
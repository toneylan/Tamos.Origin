using System;
using System.Collections.Generic;
using Tamos.AbleOrigin.Common;
using Tamos.AbleOrigin.DataProto.Payment;
using Tamos.AbleOrigin.Log;
using Tamos.AbleOrigin.Serialize;

namespace Tamos.AbleOrigin.Payment.WxPay
{
    internal class WxJsApiPayGateway : WxPayGatewayBase
    {
        //public override PayGatewayTypes GatewayType => PayGatewayTypes.WxJsApiPay;

        public WxJsApiPayGateway(CommonPayConfig config) : base(config)
        {
            
        }

        #region PayRequest

        public override PayProcessResult ProcessPayRequest(PaymentContext context)
        {
            var res = new PayProcessResult();
            //支付参数设置
            var error = CheckAndSetRequestPara(context, Trade_JSAPI, out var data);
            if (error.NotNull()) return res.Error(error);

            try
            {
                //调用统一下单接口
                var result = WxPayApi.UnifiedOrder(data);
                var retMsg = result.GetValueStr("return_msg");
                if (IsResultSuccess(result))
                {
                    //下单成功
                    res.ResultType = PayProcessResType.RequestSuccess;
                    res.AttachData = result.GetValueStr("prepay_id");
                    res.ResultValue = GetJsApiParameters(res.AttachData); //对返回结果进一步处理，以方便应用使用
                    //res.Message = retMsg;
                }
                else
                {
                    res.Error($"{retMsg}：{result.GetValueStr("err_code_des")}");
                    LogService.ErrorFormat("WxJsApiPay failure:{0}", res.ErrorMsg);
                }
                return res;
            }
            catch (Exception e)
            {
                LogService.Error(e);
                return res.Error("微信支付异常：" + e.Message);
            }
        }
        
        /// <summary>
        /// 从统一下单成功返回的数据中获取微信浏览器调起jsapi支付所需的参数，
        /// 更详细的说明请参考网页端调起支付API：http://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=7_7
        /// </summary>
        private string GetJsApiParameters(string prepayId, bool forJsSdk = false)
        {
            var jsApiParam = new WxPayData(WxPayConfig);
            jsApiParam.SetValue("appId", PayConfig.AppId);
            jsApiParam.SetValue("timeStamp", WxPayApi.GenerateTimeStamp());
            jsApiParam.SetValue("nonceStr", WxPayApi.GenerateNonceStr());
            jsApiParam.SetValue("package", "prepay_id=" + prepayId);
            jsApiParam.SetValue("signType", "MD5");
            jsApiParam.SetValue("paySign", jsApiParam.MakeSign());

            if (!forJsSdk) return jsApiParam.ToJson();
            
            //JsSdk方式调用时，参数有差别
            return SerializeUtil.ToJson(new Dictionary<string, string>
            {
                {"timestamp", jsApiParam.GetValueStr("timeStamp")},
                {"nonceStr", jsApiParam.GetValueStr("nonceStr")},
                {"package", jsApiParam.GetValueStr("package")},
                {"signType", jsApiParam.GetValueStr("signType")},
                {"paySign", jsApiParam.GetValueStr("paySign")}
            });
        }

        #endregion
        
    }
}
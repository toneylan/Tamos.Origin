using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Tamos.AbleOrigin.Common;
using Tamos.AbleOrigin.DataProto.Payment;
using Tamos.AbleOrigin.Log;

namespace Tamos.AbleOrigin.Payment
{
    public abstract class PaymentGatewayBase
    {
        protected internal PayGatewayTypes GatewayType { get; set; }
        protected CommonPayConfig PayConfig { get; }

        protected PaymentGatewayBase(CommonPayConfig payConfig)
        {
            PayConfig = payConfig;
        }

        /// <summary>
        /// 处理支付请求
        /// </summary>
        public abstract PayProcessResult ProcessPayRequest(PaymentContext context);
        
        /// <summary>
        /// 处理支付平台的结果通知
        /// </summary>
        public abstract NotifyProcessResult ProcessPayNotify(HttpContext httpContext, IOrderProcessor orderProcessor);

        /// <summary>
        /// 处理支付退款
        /// </summary>
        public virtual RefundProcessResult ProcessRefund(RefundContext context)
        {
            return new RefundProcessResult {ErrorMsg = "当前支付方式不支持退款"};
        }

        /// <summary>
        /// 查询支付结果
        /// </summary>
        public virtual PayProcessResult QueryPaymentResult(long transactionId, IOrderProcessor orderProcessor)
        {
            return new PayProcessResult().Error("未实现订单查询");
        }

        #region 读取Http请求

        /// <summary>
        /// 读取HttpRequest Body数据
        /// </summary>
        protected internal static string ReadRequestBody(HttpRequest request)
        {
            // IMPORTANT: Leave the body open so the next middleware can read it.
            using var reader = new StreamReader(request.Body, Encoding.UTF8, false, 1024, leaveOpen: true);
            var postData = reader.ReadToEndAsync().Result; //默认不支持同步方法：ReadToEnd()，需要设置AllowSynchronousIO=true;
            
            //LogService.DebugFormat("Receive Notify from :{0}", postData);
            return postData;
        }

        /// <summary>
        /// 导出Http请求信息并保存为文件
        /// </summary>
        internal static void DumpRequestToFile(HttpRequest request, string dirName)
        {
            try
            {
                //var path = HostApp.GetPath("Logs/PayNotify_" + GatewayType);
                var path = HostApp.GetPath("Logs/" + dirName);
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                path = Path.Combine(path, $"notify_{DateTime.Now:yyyy-MM-dd HH-mm-ss}.log");
                File.AppendAllText(path, DumpRequest(request), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                LogService.Error(ex);
            }
        }

        //导出Http请求的完整信息
        private static string DumpRequest(HttpRequest request)
        {
            var sb = new StringBuilder();

            //请求的客户端IP
            string clientIp = request.Headers["X-Real-IP"];
            if (clientIp.IsNull()) clientIp = request.HttpContext.Connection.RemoteIpAddress.ToString();
            sb.AppendLine("");
            sb.AppendLine("-------------  ClientInfo --------------");
            sb.AppendLine("IP Address".PadLeft(40, ' ') + " : " + clientIp);

            //post body
            request.Body.Seek(0, SeekOrigin.Begin); //前面可能被读取过一次了
            sb.AppendLine("Post Body Data".PadLeft(40, ' ') + " : " + ReadRequestBody(request));

            /* 可能报错：Content-Type: text/xml 
             sb.AppendLine("");
            sb.AppendLine("-------------  PostedParameters --------------");
            foreach (string key in request.Form.Keys)
            {
                if (string.IsNullOrEmpty(key)) continue;
                sb.AppendLine(key.PadLeft(40, ' ') + " : " + request.Form[key]);
            }*/

            sb.AppendLine("");
            sb.AppendLine("-------------  Query String ------------------");
            foreach (string key in request.Query.Keys)
            {
                sb.AppendLine(key.PadLeft(40, ' ') + " : " + request.Query[key]);
            }

            sb.AppendLine("");
            sb.AppendLine("-------------  Http Headers -------------------");
            foreach (string key in request.Headers.Keys)
            {
                sb.AppendLine(key.PadLeft(40, ' ') + " : " + request.Headers[key]);
            }

            sb.AppendLine("");
            sb.AppendLine("--------------  Cookies -------------------------");
            foreach (var ck in request.Cookies)
            {
                sb.AppendLine(ck.Key.PadLeft(40, ' ') + " : " + ck.Value);
            }

            return sb.ToString();
        }

        #endregion
    }
}

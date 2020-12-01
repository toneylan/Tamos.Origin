using System.Text;
using Microsoft.AspNetCore.Http;

namespace Tamos.AbleOrigin.Payment.WxPay
{
    /// <summary>
    /// 回调处理基类
    /// 主要负责接收微信支付后台发送过来的数据，对数据进行签名验证
    /// 子类在此类基础上进行派生并重写自己的回调处理过程
    /// </summary>
    internal class Notify
    {
        /// <summary>
        /// 接收从微信支付后台发送过来的数据并验证签名
        /// </summary>
        /// <returns>微信支付后台返回的数据</returns>
        public static WxPayData GetNotifyData(HttpRequest request, IConfig config, out string error)
        {
            //！！Net core默认不支持这样同步读取
            /*System.IO.Stream s = request.Body;
            int count = 0;
            byte[] buffer = new byte[1024];
            StringBuilder builder = new StringBuilder();
            while ((count = s.Read(buffer, 0, 1024)) > 0)
            {
                builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
            }
            s.Flush();
            s.Close();
            s.Dispose();*/

            //接收从微信后台POST过来的数据
            var bodyData = PaymentGatewayBase.ReadRequestBody(request); //Tamos update
            Log.Debug(nameof(Notify), "Receive data from WeChat : " + bodyData);

            //转换数据格式并验证签名
            WxPayData data = new WxPayData(config);
            try
            {
                data.FromXml(bodyData);
                error = null;
            }
            catch(WxPayException ex)
            {
                //若签名错误，则立即返回结果给微信支付后台
                error = ex.Message;
                Log.Error(nameof(Notify), "Sign check error : " + ex);
            }
            
            //Log.Info(this.GetType().ToString(), "Check sign success");
            return data;
        }
    }
}
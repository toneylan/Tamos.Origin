using System;
using Tamos.AbleOrigin.Common;
using Tamos.AbleOrigin.Log;
using Tamos.AbleOrigin.Serialize;

namespace Tamos.Connect.Weixin
{
    internal static class ApiServiceAgent
    {
        private static readonly DateTime UtcBaseTime = new DateTime(1970, 1, 1);

        public static DateTime ParseDateTime(long dateTicks)
        {
            return UtcBaseTime.AddSeconds(dateTicks);
        }

        private static T Deserialize<T>(string source, out ErrorDes errDes)
        {
            errDes = null;
            if (string.IsNullOrEmpty(source)) return default;

            //错误时微信会返回JSON数据包如下（示例为Code无效错误）:
            //{"errcode":40029,"errmsg":"invalid code"}
            if (!source.Contains("errcode")) return SerializeUtil.FromJson<T>(source);
            
            errDes = SerializeUtil.FromJson<ErrorDes>(source);
            return default;
        }

        public static T CallAPI<T>(string url, out ErrorDes errDes)
        {
            var resStr = WebReqUtil.GetRequest(url, out var error);
            T apiRes;
            if (error.NotNull())
            {
                apiRes = default;
                errDes = new ErrorDes {errmsg = error};
            }
            else
            {
                apiRes = Deserialize<T>(resStr, out errDes);
            }

            if (errDes != null) LogService.ErrorFormat("Call 微信Api error:{0}, Res:{1}, Url:{2}", errDes.errmsg, resStr, url);
            return apiRes;
        }

        public static T CallAPI<T>(string url, string jsonPara, out ErrorDes errDes)
        {
            var resStr = WebReqUtil.PostRequest(url, jsonPara, out var error, WebReqUtil.Content_Json);
            T apiRes;

            if (error.NotNull())
            {
                apiRes = default;
                errDes = new ErrorDes { errmsg = error };
            }
            else
            {
                apiRes = Deserialize<T>(resStr, out errDes);
            }

            if (errDes != null) LogService.ErrorFormat("Call 微信Api error:{0}, Res:{1}, Url:{2}", errDes.errmsg, resStr, url);
            return apiRes;
        }
    }
}
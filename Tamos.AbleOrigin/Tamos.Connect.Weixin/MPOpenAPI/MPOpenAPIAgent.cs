using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Tamos.AbleOrigin.Common;
using Tamos.AbleOrigin.IOC;
using Tamos.AbleOrigin.Log;
using Tamos.AbleOrigin.Security;

namespace Tamos.Connect.Weixin
{
    public class MpOpenAPIAgent
    {
        public WechatAcountConf CurMPAccount { get; }

        public MpOpenAPIAgent(string appId, string appSecret)
        {
            CurMPAccount = new WechatAcountConf(appId, appSecret);
        }

        public MpOpenAPIAgent(WechatAcountConf accountConf)
        {
            CurMPAccount = accountConf;
        }

        #region Access Token

        //避免重复请求Token造成冲突
        private static readonly object TokenRefLock = new object();

        internal string GetAccessToken()
        {
            var token = GetAccessTokenSync();
            if (token == null) return string.Empty;

            return token.Token;
        }

        public AccessToken GetAccessTokenSync()
        {
            var connectProvider = ServiceLocator.GetInstance<IWeixinConnectProvider>();
            if (connectProvider == null) throw new Exception("未注册IWeixinConnectProvider接口的实现类");
            var token = connectProvider.GetAccessToken(CurMPAccount.AppId);
            if (token != null && DateTime.Now < token.ExpireTime) return token;

            //单线程执行
            lock (TokenRefLock)
            {
                //检查是否已被其他线程刷新
                token = connectProvider.GetAccessToken(CurMPAccount.AppId);
                if (token != null && DateTime.Now < token.ExpireTime) return token;

                //get the new token
                var url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={CurMPAccount.AppId}&secret={CurMPAccount.AppSecret}";
                string error = null;
                var resStr = WebReqUtil.GetRequest(url, out error);
                if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(resStr))
                {
                    LogService.ErrorFormat("get access token error:{0}", error);
                    return null;
                }
                LogService.DebugFormat("get access token result:{0}", resStr);

                var resObj = JsonConvert.DeserializeObject<ErrorDes>(resStr);
                if (resObj.errcode > 0) LogService.DebugFormat("get access token failed:{0}", resStr);

                var res = JsonConvert.DeserializeAnonymousType(resStr, new { access_token = string.Empty, expires_in = 0 });
                if (res == null) return null;
                //store new token
                token = new AccessToken
                {
                    AppId = CurMPAccount.AppId,
                    Token = res.access_token,
                    ExpireTime = DateTime.Now.AddSeconds(res.expires_in)
                };
                connectProvider.StoreAccessToken(token);
            }

            return token;
        }

        #endregion

        #region jsApi

        //避免重复请求Token造成冲突
        private static readonly object JsTicketRefLock = new object();

        public string GetJsApiTicket()
        {
            var connectProvider = ServiceLocator.GetInstance<IWeixinConnectProvider>();
            if (connectProvider == null) throw new Exception("未注册IWeixinConnectProvider接口的实现类");
            var token = connectProvider.GetJsApiTicket(CurMPAccount.AppId);
            if (token != null && DateTime.Now < token.ExpireTime) return token.Token;

            //单线程执行
            lock (JsTicketRefLock)
            {
                //检查是否已被其他线程刷新
                token = connectProvider.GetJsApiTicket(CurMPAccount.AppId);
                if (token != null && DateTime.Now < token.ExpireTime) return token.Token;

                //get the new ticket
                var url = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", GetAccessToken());
                string error;
                var resStr = WebReqUtil.GetRequest(url, out error);
                if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(resStr))
                {
                    LogService.ErrorFormat("Wechat fail get js ticket:{0}", error);
                    return null;
                }
                var resObj = JsonConvert.DeserializeObject<ErrorDes>(resStr);
                if (resObj.errcode > 0) LogService.DebugFormat("Wechat fail get js ticket:{0}", resStr);

                var res = JsonConvert.DeserializeAnonymousType(resStr, new { ticket = string.Empty, expires_in = 0 });
                if (res == null) return null;
                //store new ticket
                token = new AccessToken
                {
                    AppId = CurMPAccount.AppId,
                    Token = res.ticket,
                    ExpireTime = DateTime.Now.AddSeconds(res.expires_in)
                };
                connectProvider.StoreJsApiTicket(token);
            }

            return token.Token;
        }

        /// <summary>
        /// 获取JsApiConfig (需要在 公众号设置》功能设置》JS安全域名，否则可能会出现无法获取Tekon=null和签名错误)
        /// </summary>
        /// <param name="url">当前网页的URL，不包含#及其后面部分</param>
        /// <param name="apiList"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public string GetJsApiConfig(string url, List<string> apiList, bool debug = false)
        {
            var nonceStr = Utility.BuildRandomStr(16);
            var timestamp = GetUnixTimestamp();
            //待签名参数按字段名的ASCII 码从小到大排序（字典序）后，使用URL键值对的格式（即key1=value1&key2=value2…）拼接成字符串
            var signSrc = $"jsapi_ticket={GetJsApiTicket()}&noncestr={nonceStr}&timestamp={timestamp}&url={url}";
            var confObj = new
            {
                debug, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，参数信息会通过log打出，仅在pc端时才会打印。
                appId = CurMPAccount.AppId,
                timestamp, // 必填，生成签名的时间戳
                nonceStr, // 必填，生成签名的随机串
                signature = Encryptor.SHA1Encrypt(signSrc), // 必填，签名，校验地址：https://mp.weixin.qq.com/debug/cgi-bin/sandbox?t=jsapisign
                jsApiList = apiList // 必填，需要使用的JS接口列表，所有JS接口列表见附录2   
            };

            if (debug)
            {
                LogService.DebugFormat("GetJsApiConfig signSrc:{0}", signSrc);
                LogService.DebugFormat("GetJsApiConfig confObj:{0}", JsonConvert.SerializeObject(confObj));
            }

            return JsonConvert.SerializeObject(confObj);
        }

        #endregion

        public UserInfoObj GetUserInfo(string openId)
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN",
                GetAccessToken(), openId);

            ErrorDes error;
            var user = ApiServiceAgent.CallAPI<UserInfoObj>(url, out error);
            if (!string.IsNullOrEmpty(error?.errmsg)) LogService.ErrorFormat("Wechat get user info error:{0}", error.errmsg);
            return user;
        }

        #region Template message

        public string SendTemplateMsg(TemplateMsgSendPara para)
        {
            var url = string.Concat("https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=", GetAccessToken());
            var paraStr = JsonConvert.SerializeObject(para);

            ErrorDes error;
            var res = ApiServiceAgent.CallAPI<ErrorDes>(url, paraStr, out error);
            if (res == null) return "微信接口调用失败:" + error;
            return res.errcode == 0 ? null : "微信接口调用失败:" + res.errmsg;
        }

        #endregion

        #region Util method

        private string GetUnixTimestamp()
        {
            return ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString();
        }

        #endregion
    }
}
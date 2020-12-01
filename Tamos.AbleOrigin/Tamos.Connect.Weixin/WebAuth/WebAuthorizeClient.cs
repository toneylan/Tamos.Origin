using System.Web;

namespace Tamos.Connect.Weixin
{
    public class WebAuthorizeClient
    {
        private WechatAcountConf _wechatAcountConf;
        public const string CodeQueryParaName = "code";

        public WebAuthorizeClient(string appId, string appSecret)
        {
            _wechatAcountConf = new WechatAcountConf(appId, appSecret);
        }

        public static string BuildAuthorizeUrl(string appId, string retUrl, string state = null, bool userinfoScope = false)
        {
            return string.Format(
                "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope={2}&state={3}#wechat_redirect"
                , appId, HttpUtility.UrlEncode(retUrl), userinfoScope ? "snsapi_userinfo" : "snsapi_base", state);
        }

        public WebAuthRes GetAccessToken(string code, out string error)
        {
            var url = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code",
                _wechatAcountConf.AppId, _wechatAcountConf.AppSecret, code);

            var res = ApiServiceAgent.CallAPI<WebAuthRes>(url, out var errDes);
            error = errDes?.errmsg;
            //isReUse = errDes != null && errDes.errcode == 40163;
            
            return res;
        }

        public UserInfoObj GetUserInfo(string accessToken, string openId, out string error)
        {
            var url = string.Format("https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN", accessToken, openId);
            //返回数据：
            //{
            //    "openid": "o8V571O4OpXS9SvKAw9x1U9pl4i4",
            //    "nickname": "404",
            //    "sex": 1,
            //    "language": "zh_CN",
            //    "city": "",
            //    "province": "尼古拉耶夫",
            //    "country": "乌克兰",
            //    "headimgurl": "http:\/\/thirdwx.qlogo.cn\/mmopen\/vi_32\/6LMUKXmCOeYTBorejJGehztNqeNLt69SiaYRtYeYOQo2ZqcDFUlGINpj01QfhEcXc4ia7r4qdxGZiawEv0FUphiatg\/132",
            //    "privilege": []
            //}

            var res = ApiServiceAgent.CallAPI<UserInfoObj>(url, out var errDes);
            error = errDes?.errmsg;
            return res;
        }
    }
}
using System.Web;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.Connect.Weixin;

public class WebAuthorizeClient : WxApiClient
{
    public const string CodeQueryParaName = "code";

    public WebAuthorizeClient(string appId, string appSecret) : base(appId, appSecret)
    { }

    public static string BuildAuthorizeUrl(string appId, string retUrl, string? state = null, bool userinfoScope = false)
    {
        return string.Format(
            "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope={2}&state={3}#wechat_redirect"
            , appId, HttpUtility.UrlEncode(retUrl), userinfoScope ? "snsapi_userinfo" : "snsapi_base", state);
    }

    //网页授权是单独的Token，跟其他接口不一样。
    public Task<GeneralRes<WebAuthRes>> GetAccessToken(string code)
    {
        var url = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code",
            Acount.AppId, Acount.AppSecret, code);

        var res = GetPure<WebAuthRes>(url);

        //isReUse = errDes != null && errDes.errcode == 40163;

        return res;
    }

    public Task<GeneralRes<WxUserInfo>> GetUserInfo(string accessToken, string openId)
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
        //    "privilege": [],
        //    "unionid": "o6_bmasdasdsad6_2sgVt7hMZOPfL"
        //}

        return GetPure<WxUserInfo>(url);
    }
}
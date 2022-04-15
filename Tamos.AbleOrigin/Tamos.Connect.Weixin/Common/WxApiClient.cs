using Tamos.AbleOrigin.DataProto;

namespace Tamos.Connect.Weixin;

public class WxApiClient
{
    internal WxAcountSet Acount { get; }

    public WxApiClient(string appId, string appSecret)
    {
        Acount = new WxAcountSet(appId, appSecret);
    }

    #region Access Token

    /// <summary>
    /// 获取全局唯一接口调用凭据
    /// </summary>
    internal string GetToken()
    {
        var key = "WeixinAccessToken:" + Acount.AppId;
        var token = CacheService.Get<WxAccessToken>(key);
        if (token?.IsValid() == true) return token.access_token;

        //不能重复获取，故单例进入
        var getLock = DistributedService.RunInLock(key, TimeSpan.FromMinutes(1), TimeSpan.Zero, () =>
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={Acount.AppId}&secret={Acount.AppSecret}";
            token = GetPure<WxAccessToken>(url).Result.Data;
            if (token == null) return;

            // 保存到Cache中
            token.Expire = DateTime.Now.AddSeconds(token.expires_in);
            CacheService.Set(key, token, TimeSpan.FromSeconds(token.expires_in));
        });
        if (getLock) return token?.access_token ?? string.Empty;

        //-- 并发实例，轮询获取
        var waitTime = 0;
        do
        {
            Thread.Sleep(2000);
            token = CacheService.Get<WxAccessToken>(key);
            if (token == null || !token.IsValid()) continue;

            LogService.InfoFormat("微信AccessToken并发获取，等待了{0}秒", (waitTime + 1) * 2);
            break;
        } while (++waitTime <= 5);

        return token?.access_token ?? string.Empty;
    }

    #endregion

    #region Api call util

    internal static async Task<string?> Post(string url, string content)
    {
        var res = await Post<WxResultBase>(url, content);

        if (res?.IsSuccess() == true) return null;
        return string.IsNullOrEmpty(res?.errmsg) ? "微信Api error: null res" : res.errmsg;
    }

    internal static async Task<T?> Post<T>(string url, string content) where T : WxResultBase
    {
        var resStr = await HttpUtil.PostAsync(url, content);
        var res = SerializeUtil.FromJson<T>(resStr);

        if (res == null || !res.IsSuccess()) LogService.ErrorFormat("微信Api error:{0}, Res:{1}, Url:{2}", res?.errmsg!, resStr, url);
        return res;
    }

    /// <summary>
    /// 正常请求结果不能包含：errcode
    /// </summary>
    internal static async Task<GeneralRes<T>> GetPure<T>(string url)
    {
        var res = new GeneralRes<T>();
        try
        {
            var resStr = await HttpUtil.GetStringAsync(url);
            res.Data = DeserializePure<T>(resStr, out var errMsg);

            if (errMsg.IsNull()) return res;

            LogService.ErrorFormat("微信Api error:{0}, Res:{1}, Url:{2}", errMsg, resStr, url);
            return res.On(errMsg);
        }
        catch (Exception e)
        {
            LogService.Error(e);
            return res.On(e.Message);
        }
    }

    /// <summary>
    /// 类型T不会带通用的errcode属性。
    /// </summary>
    private static T DeserializePure<T>(string source, out string? errMsg)
    {
        errMsg = null;
        if (string.IsNullOrEmpty(source)) return default;

        //错误时微信会返回JSON数据包如下（示例为Code无效错误）:
        //{"errcode":40029,"errmsg":"invalid code"}
        if (!source.Contains("errcode")) return SerializeUtil.FromJson<T>(source);

        errMsg = SerializeUtil.FromJson<WxResultBase>(source)?.errmsg;
        return default;
    }

    #endregion
}
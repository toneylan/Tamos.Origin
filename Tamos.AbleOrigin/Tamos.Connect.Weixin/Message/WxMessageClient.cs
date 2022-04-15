
namespace Tamos.Connect.Weixin;

public class WxMessageClient : WxApiClient
{
    public WxMessageClient(string appId, string appSecret) : base(appId, appSecret)
    { }

    #region Template message

    /// <summary>
    /// 发送模板消息，确保公众号已开通模板消息，并添加了对应的消息模板。
    /// </summary>
    public Task<string?> SendTemplateMsgAsync(TemplateMsgPara para)
    {
        var url = $"https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={GetToken()}";
        var paraStr = SerializeUtil.ToJson(para);

        return Post(url, paraStr);
    }

    #endregion
}
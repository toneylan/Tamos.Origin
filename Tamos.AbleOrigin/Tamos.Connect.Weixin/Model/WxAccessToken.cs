namespace Tamos.Connect.Weixin;

internal class WxAccessToken
{
    public string access_token { get; set; }

    /// <summary>
    /// 凭证有效时间，单位：秒
    /// </summary>
    public int expires_in { get; set; }

    /// <summary>
    /// 非返回属性，增加的以便保存。
    /// </summary>
    public DateTime Expire { get; set; }

    internal bool IsValid() => Expire > DateTime.Now;
}
namespace Tamos.Connect.Weixin
{
    /// <summary>
    /// 公众号配置信息
    /// </summary>
    internal class WxAcountSet
    {
        public WxAcountSet(string appId, string appSecret)
        {
            AppId = appId;
            AppSecret = appSecret;
        }

        public string AppId { get; }
        public string AppSecret { get; }
    }
}
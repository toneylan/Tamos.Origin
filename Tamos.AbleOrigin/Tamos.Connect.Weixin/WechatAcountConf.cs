namespace Tamos.Connect.Weixin
{
    /// <summary>
    /// 公众号配置信息
    /// </summary>
    public class WechatAcountConf
    {
        public WechatAcountConf(string appId, string appSecret)
        {
            AppId = appId;
            AppSecret = appSecret;
        }

        //public string AccountName { get; private set; }
        public string AppId { get; private set; }
        public string AppSecret { get; private set; }
        //public string Token { get; private set; }
    }
}
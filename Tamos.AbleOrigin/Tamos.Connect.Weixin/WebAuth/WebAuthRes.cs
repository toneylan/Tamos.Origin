namespace Tamos.Connect.Weixin
{
    public class WebAuthRes
    {
        public string access_token { get; set; }
        
        /// <summary>
        /// access_token接口调用凭证超时时间，单位（秒）
        /// </summary>
        public int expires_in { get; set; }

        public string refresh_token { get; set; }
        public string openid { get; set; }
        public string? scope { get; set; }
    }
}
using System;

namespace Tamos.Connect.Weixin
{
    [Serializable]
    public class AccessToken
    {
        public string AppId { get; set; }
        public string Token { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}
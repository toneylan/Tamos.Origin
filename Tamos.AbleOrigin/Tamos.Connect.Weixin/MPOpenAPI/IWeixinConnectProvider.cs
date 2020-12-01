namespace Tamos.Connect.Weixin
{
    public interface IWeixinConnectProvider
    {
        AccessToken GetAccessToken(string appId);

        void StoreAccessToken(AccessToken token);

        AccessToken GetJsApiTicket(string appId);

        void StoreJsApiTicket(AccessToken token);
    }
}
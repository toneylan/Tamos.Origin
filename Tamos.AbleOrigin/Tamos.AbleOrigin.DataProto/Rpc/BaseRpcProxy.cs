namespace Tamos.AbleOrigin.DataProto
{
    /// <summary>
    /// Rpc服务代理
    /// </summary>
    public class BaseRpcProxy<TContract> where TContract : class
    {
        /*protected virtual CallContext Context => CallContext.Default;*/

        /// <summary>
        /// Client是线程安全的。https://docs.microsoft.com/en-us/aspnet/core/grpc/client
        /// </summary>
        protected TContract Client { get; }

        protected BaseRpcProxy()
        {
            Client = (_defaultChannel ??= RpcChannelFactory.CreateChannel(_serverAddress!)).CreateClient<TContract>();
        }

        /// <summary>
        /// 动态配置服务地址
        /// </summary>
        protected BaseRpcProxy(string serverAddress)
        {
            Client = RpcChannelFactory.CreateChannel(serverAddress).CreateClient<TContract>();
        }

        #region Static member

        // ReSharper disable StaticMemberInGenericType
        private static string? _serverAddress;
        
        //定义TContract类型的默认Channel，按需创建，避免ConfigServer时初始化。
        private static BaseRpcChannel? _defaultChannel;

        /// <summary>
        /// 全局配置<typeparamref name="TContract"/>对应服务的地址：IP + Port
        /// </summary>
        protected static void ConfigServer(string serverAddress)
        {
            _serverAddress = serverAddress;
        }

        #endregion
    }
}
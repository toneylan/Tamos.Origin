// ReSharper disable StaticMemberInGenericType

namespace Tamos.AbleOrigin.Booster
{
    /// <summary>
    /// Grpc服务代理
    /// </summary>
    public abstract class BaseGrpcServiceProxy<TContract> where TContract : class
    {
        #region SetProxy

        private static BaseGrpcChannel _channel;
        
        /*/// <summary>
        /// 代理设置
        /// </summary>
        public static void SetProxy(BaseGrpcChannel channel)
        {
            _channel = channel;
        }*/

        /// <summary>
        /// 设置服务地址(IP:Port)，可通过BaseGrpcChannel.CreateChannel自定义client模式。
        /// </summary>
        public static void SetProxy(string hostAddress)
        {
            _channel = BaseGrpcChannel.ChannelBuilder?.Invoke(hostAddress) ?? new NativeGrpcChannel(hostAddress);
        }

        #endregion

        /// <summary>
        /// Client也是线程安全的。https://docs.microsoft.com/en-us/aspnet/core/grpc/client
        /// </summary>
        protected TContract Client { get; } = _channel.CreateGrpcService<TContract>();
    }
}
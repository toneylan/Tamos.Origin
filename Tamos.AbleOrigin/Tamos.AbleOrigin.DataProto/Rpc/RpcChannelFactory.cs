
namespace Tamos.AbleOrigin.DataProto
{
    /// <summary>
    /// 抽象ChannelFactory，Factory自身会使用Singleton。
    /// </summary>
    public abstract class RpcChannelFactory
    {
        //private static readonly RpcChannelFactory ChannelFactory = ServiceLocator.GetInstance<RpcChannelFactory>();

        /// <summary>
        /// 获取指定地址Service的Channel。
        /// </summary>
        public abstract BaseRpcChannel GetChannel(string address);


        internal static BaseRpcChannel CreateChannel(string address)
        {
            return ServiceLocator.GetInstance<RpcChannelFactory>().GetChannel(address);
        }
    }

    /// <summary>
    /// 抽象Rpc Channel，用于Service client的创建。
    /// </summary>
    public abstract class BaseRpcChannel
    {
        /// <summary>
        /// 创建Rpc service client
        /// </summary>
        public abstract T CreateClient<T>() where T : class;
        
    }
}
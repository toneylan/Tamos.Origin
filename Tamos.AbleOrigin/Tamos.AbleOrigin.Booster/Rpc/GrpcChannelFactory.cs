using System;
using System.Collections.Generic;
using Grpc.Core;
using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.Booster
{
    /// <summary>
    /// 管理GrpcChannel的创建(Use Singleton)。
    /// </summary>
    internal class GrpcChannelFactory : RpcChannelFactory, IDisposable
    {
        //单例channel在高并发下，可能有性能问题，.net 5之后默认已开启EnableMultipleHttp2Connections = true 来解决此问题，目前未做实际测试。
        //https://docs.microsoft.com/en-us/aspnet/core/grpc/performance?view=aspnetcore-5.0#connection-concurrency

        private static readonly Dictionary<string, GrpcChannelWrapper> ChannelCache = new();

        public override BaseRpcChannel GetChannel(string address)
        {
            if (ChannelCache.TryGetValue(address, out var channel)) return channel;

            channel = new GrpcChannelWrapper(GrpcChannel.ForAddress($"http://{address}"));
            ServiceLocator.Container.RecordInAppLife(channel);

            return ChannelCache.SetValue(address, channel);
        }

        public void Dispose()
        {
            ChannelCache.ForEach(x => x.Value.Dispose());
        }
    }

    #region GrpcChannelWrapper

    internal class GrpcChannelWrapper : BaseRpcChannel, IDisposable
    {
        internal GrpcChannel InnerChannel { get; }

        internal GrpcChannelWrapper(GrpcChannel channel)
        {
            InnerChannel = channel;
        }

        /// <summary>
        /// 创建Grpc服务客户端
        /// </summary>
        public override T CreateClient<T>()
        {
            try
            {
                return InnerChannel.CreateGrpcService<T>();
            }
            catch (Exception e)
            {
                LogService.Error(e);
                return null!;
            }
        }

        public void Dispose()
        {
            InnerChannel.Dispose();
        }
    }

    #endregion
}
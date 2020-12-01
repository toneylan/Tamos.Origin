using System;
using Grpc.Core;
using ProtoBuf.Grpc.Client;
using Tamos.AbleOrigin.Common;
using Tamos.AbleOrigin.Log;

namespace Tamos.AbleOrigin.Booster
{
    /// <summary>
    /// 管理Grpc客户端的创建。可用Grpc native 或者 Grpc.Net.Client模式。
    /// </summary>
    public abstract class BaseGrpcChannel
    {
        protected ChannelBase Channel { get; set; }

        /// <summary>
        /// 创建Grpc服务客户端
        /// </summary>
        public virtual T CreateGrpcService<T>() where T : class
        {
            try
            {
                return Channel.CreateGrpcService<T>();
            }
            catch (Exception e)
            {
                LogService.Error(e);
                return null;
            }
        }

        #region Static Channel create

        /// <summary>
        /// Channel创建函数
        /// </summary>
        internal static Func<string, BaseGrpcChannel> ChannelBuilder { get; private set; }

        /// <summary>
        /// 自定义Channel创建（用于配置Net.Client模式）
        /// </summary>
        public static void UseBuilder(Func<string, BaseGrpcChannel> builderFunc)
        {
            ChannelBuilder = builderFunc;
        }

        public static (string host, int port) ParseAddress(string address)
        {
            var ipPort = address.Split(':');
            return (ipPort[0], ipPort[1].ToInt());
        }

        #endregion
    }

    /// <summary>
    /// Grpc native模式，非托管代码实现。(.net framework使用)
    /// </summary>
    internal class NativeGrpcChannel : BaseGrpcChannel
    {
        public NativeGrpcChannel(string hostAddress)
        {
            var (host, port) = ParseAddress(hostAddress);
            Channel = new Channel(host, port, ChannelCredentials.Insecure);
        }

        /*#region Server

        private static List<Server> _servers;

        /// <summary>
        /// 启动Grpc服务端
        /// </summary>
        protected internal static void StartServer<TService>(string hostAddress, TService service) 
            where TService : class
        {
            //BaseModel.TypeInit();
            if (_servers == null)
            {
                FrameInitializer.AppExiting += StopServers;
                _servers = new List<Server>(1);
            }

            var address = ParseAddress(hostAddress);
            const string host = "0.0.0.0"; //绑定到所有地址
            var server = new Server
            {
                Ports = {new ServerPort(host, address.Item2, ServerCredentials.Insecure)}
            };
            server.Services.AddCodeFirst(service, null, new LogTextWriter(), new[] {new GrpcIocInterceptor()});
            server.Start();

            _servers.Add(server);

            LogService.DebugFormat("{0} listening on:{1}:{2}", typeof(TService).Name, host, address.Item2);
        }

        /// <summary>
        /// 停止所有启动的Server
        /// </summary>
        private static void StopServers()
        {
            _servers?.ForEach(x => x.ShutdownAsync().Wait());
        }

        #endregion*/
    }
}
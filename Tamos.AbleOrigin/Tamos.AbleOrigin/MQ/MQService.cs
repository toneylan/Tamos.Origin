using System;

namespace Tamos.AbleOrigin
{
    /// <summary>
    /// 封装消息队列服务，Standalone应用可能无法使用。<br/>
    /// 注意：在完成IOC Register后再使用。添加Package RabbitMQ.Client
    /// </summary>
    public static class MQService
    {
        private static readonly IMQProvider? _provider = MQProviderFactory.GetProvider();
        public static IMQProvider Provider => _provider ?? throw new Exception("没有注册MQ服务");


        /// <inheritdoc cref="IMQProvider.Publish{T}" />
        public static void Publish<T>(string queueName, T content)
        {
            Provider.Publish(queueName, content);
        }
    }

    public static class MQProviderFactory
    {
        private static Func<IMQProvider>? ProviderBuilder;

        /// <summary>
        /// 避免IOC注册Singleton后，会创建实例，引起不使用MQ项目，缺少client dll。
        /// </summary>
        public static void AddMQProvider<T>(this IServiceContainer _) where T : IMQProvider, new()
        {
            ProviderBuilder = () => new T();
        }

        internal static IMQProvider? GetProvider()
        {
            return ProviderBuilder != null ? ProviderBuilder() : ServiceLocator.GetOrReflect<IMQProvider>("RabbitMQProvider");
        } 
    }
}
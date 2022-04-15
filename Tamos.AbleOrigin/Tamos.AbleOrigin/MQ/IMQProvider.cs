using System;

namespace Tamos.AbleOrigin
{
    public interface IMQProvider
    {
        /// <summary>
        /// 定义通用的基础组件，应用系统无需调用。
        /// </summary>
        void DeclareCommonInfra();

        /// <summary>
        /// 执行队列等组件定义。
        /// </summary>
        void DeclareSchema(Action<IMQProducer> declareAction);

        /// <summary>
        /// 创建一个Producer
        /// </summary>
        IMQProducer CreateProducer(bool logMessage = false);

        /// <summary>
        /// 创建一个Consumer
        /// </summary>
        IMQConsumer CreateConsumer<T>(string queueName, Func<T, MQHandleRes> handler);

        /// <summary>
        /// 通过默认Producer向队列发送消息
        /// </summary>
        void Publish<T>(string queueName, T content);
    }
}
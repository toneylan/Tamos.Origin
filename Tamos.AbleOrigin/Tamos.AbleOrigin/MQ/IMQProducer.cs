namespace Tamos.AbleOrigin
{
    public interface IMQProducer
    {
        /// <summary>
        /// 是否记录投递消息Log
        /// </summary>
        bool LogMessage { get; set; }

        /// <summary>
        /// exchange设置string.Empty，则为默认的direct，routingKey即为目标Queue。
        /// </summary>
        void Publish<T>(string exchange, string routingKey, T content);

        /// <summary>
        /// 发送延迟消息，会在delayTime后投递到toQueue。
        /// </summary>
        void PublishDelay<T>(MQDelayTime delayTime, string toQueue, T content);

        /// <summary>
        /// 确保创建了一个持久化队列
        /// </summary>
        void DeclareQueue(string queueName);
    }
}
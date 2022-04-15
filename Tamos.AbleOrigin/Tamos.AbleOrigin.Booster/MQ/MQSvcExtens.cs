using System;
using RabbitMQ.Client;

namespace Tamos.AbleOrigin.Booster
{
    public static class MQSvcExtens
    {
        /// <summary>
        /// 发送到延迟队列，依靠超时实现。 https://www.rabbitmq.com/dlx.html
        /// </summary>
        internal static void PublishDelay(this IModel channel, MQDelayTime time, string toQueue, IBasicProperties? props, ReadOnlyMemory<byte> body)
        {
            var exchange = MQConfig.DelayExchange(time);

            //delay exchange是topic类型，任何routing key都进对应的延迟队列。
            //到时间（超时）后，利用死信提交到 toQueue。
            channel.BasicPublish(exchange, toQueue, true, props, body);
        }


    }
}
using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Tamos.AbleOrigin.Booster
{
    /// <summary>
    /// 消息生产者基类
    /// </summary>
    internal class MQProducer : IMQProducer, IDisposable
    {
        internal IModel Channel { get; }
        public bool LogMessage { get; set; }
        
        internal MQProducer(IModel model)
        {
            Channel = model;
            Channel.BasicReturn += Channel_BasicReturn;
        }
        
        /// <summary>
        /// 没命名为QueueDeclare，避免与IModel.QueueDeclare扩展方法混淆。
        /// </summary>
        public void DeclareQueue(string queueName)
        {
            Channel.QueueDeclare(queueName, true, false, false);
        }

        #region Publish

        /// <inheritdoc />
        public void Publish<T>(string exchange, string routingKey, T content)
        {
            DoPublish(routingKey, content, (props, body) =>
                Channel.BasicPublish(exchange, routingKey, true, props, body));
        }
        
        public void PublishDelay<T>(MQDelayTime delayTime, string toQueue, T content)
        {
            DoPublish(toQueue, content, (props, body) =>
                Channel.PublishDelay(delayTime, toQueue, props, body));
        }

        private void DoPublish<T>(string routingKey, T content, Action<IBasicProperties, ReadOnlyMemory<byte>> pubAction)
        {
            try
            {
                if (content == null)
                {
                    LogService.ErrorFormat("MQ Publish null msg:{0}", routingKey);
                    return;
                }

                //统一将消息序列化为json格式，方便管理查看。
                var jsonContent = SerializeUtil.ToJson(content);
                //日志记录
                if (LogMessage) LogService.DebugFormat("MQ pub to {0}: {1}", routingKey, jsonContent);

                //Msg prop
                var prop = Channel.CreateBasicProperties();
                prop.Persistent = true;
                var bodyData = Encoding.UTF8.GetBytes(jsonContent);

                pubAction(prop, bodyData);
            }
            catch (Exception e)
            {
                LogService.Error(e);
            }
        }

        #endregion

        #region Event、Dispose
        
        private void Channel_BasicReturn(object sender, BasicReturnEventArgs e)
        {
            LogService.ErrorFormat("MQ BasicReturn Exchange:{0}, RoutingKey:{1}, Reply:{2}-{3}",
                e.Exchange, e.RoutingKey, e.ReplyCode, e.ReplyText);
        }

        public void Dispose()
        {
            try
            {
                Channel.Dispose();
            }
            catch (Exception e)
            {
                LogService.Error(e);
            }
        }

        #endregion
    }
}
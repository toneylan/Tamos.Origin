using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Tamos.AbleOrigin.Booster
{
    internal class MQConsumer<T> : IMQConsumer, IDisposable
    {
        internal IModel Channel { get; }

        /// <inheritdoc />
        public int MaxRetry { get; set; } = 3;

        private readonly string _consumeQueue;
        private readonly EventingBasicConsumer _innerConsumer;

        internal Func<T, MQHandleRes> MessageHandler { get; set; }

        #region Ctor

        public MQConsumer(IModel channel, string queue)
        {
            Channel = channel;
            _consumeQueue = queue;

            _innerConsumer = new EventingBasicConsumer(channel); //异步处理则用AsyncEventingBasicConsumer
            _innerConsumer.Received += Consumer_Received;

            Channel.BasicConsume(queue, false, _innerConsumer);
        }

        #endregion

        #region Handle received

        private const string PropKeyRetry = Utility.ProdBrand + "_retry";

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            #region Handle msg

            MQHandleRes handleRes;
            string msgContent = null;
            try
            {
                msgContent = Encoding.UTF8.GetString(e.Body.Span);
                handleRes = MessageHandler(SerializeUtil.FromJson<T>(msgContent));
            }
            catch (Exception ex)
            {
                LogService.ErrorFormat("MQ consumer fail: {0}\n{1}", msgContent!, ex);
                handleRes = MQHandleRes.FailRetry;
            }

            #endregion

            #region Reply MQ broker

            try
            {
                //发送回执
                if (handleRes == MQHandleRes.Success) Channel.BasicAck(e.DeliveryTag, false); //确认消息已处理
                else Channel.BasicReject(e.DeliveryTag, false); //丢弃或进死信

                if (handleRes != MQHandleRes.FailRetry) return;

                #region Retry by Delay Delivery

                //已重试次数
                var prop = e.BasicProperties;
                object retryObj = null;
                var retryCount = prop.Headers?.TryGetValue(PropKeyRetry, out retryObj) == true ? (int)retryObj : 0;

                //超限则入死信记录
                if (retryCount >= MaxRetry)
                {
                    Channel.BasicPublish(string.Empty, MQConfig.CommonDl, false, prop, e.Body);
                    LogService.ErrorFormat("MQ consumer out MaxRetry({0}): {1}", retryCount, msgContent);
                    return;
                }
                LogService.WarnFormat("MQ consumer has Retry({0}): {1}", retryCount, msgContent);

                //发送到延迟队列
                if (prop.Headers == null) prop.Headers = new Dictionary<string, object>();
                else prop.Headers.Remove("x-death"); //清除dl机制的标记，否则会累积多个。
                prop.Headers[PropKeyRetry] = ++retryCount;

                Channel.PublishDelay(MQDelayTime.Min10, _consumeQueue, prop, e.Body);

                #endregion
            }
            catch (Exception ex)
            {
                LogService.Error(ex);
            }

            #endregion
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            try
            {
                if (_innerConsumer.ConsumerTags != null)
                {
                    foreach (var tag in _innerConsumer.ConsumerTags)
                    {
                        Channel.BasicCancel(tag);
                    }
                }

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
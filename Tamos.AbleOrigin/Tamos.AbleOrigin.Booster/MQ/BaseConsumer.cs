using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Tamos.AbleOrigin.Common;
using Tamos.AbleOrigin.Log;

namespace Tamos.AbleOrigin.Booster
{
    public class BaseConsumer : IDisposable
    {
        /// <summary>
        /// 失败时最大重新投递次数，超过则失败（死信）
        /// </summary>
        internal int MaxReDelivery { get; set; }

        internal IModel Channel { get; private set; }
        private EventingBasicConsumer _innerConsumer;
        private string _consumeQueue;

        internal Func<string, MsgHandleRes> MessageHandler { get; set; }

        #region Init

        public BaseConsumer()
        {
            MaxReDelivery = 5; //默认重试5次
        }

        internal void Init(IModel channel, string queue)
        {
            Channel = channel;
            _consumeQueue = queue;
            _innerConsumer = new EventingBasicConsumer(channel);
            _innerConsumer.Received += _basicConsumer_Received;

            Channel.BasicConsume(queue, false, _innerConsumer);
        }

        #endregion

        private void _basicConsumer_Received(object sender, BasicDeliverEventArgs e)
        {
            //执行处理逻辑
            var handleRes = MsgHandleRes.Success;
            string msgData = null;
            try
            {
                msgData = Encoding.UTF8.GetString(e.Body.ToArray());
                if (MessageHandler != null) handleRes = MessageHandler(msgData);
            }
            catch (Exception ex)
            {
                LogService.ErrorFormat("MQ消息“{0}”处理失败：{1}", msgData, ex);
                handleRes = MsgHandleRes.FailAndReQueue;
            }

            #region 发送回执

            try
            {
                if (handleRes == MsgHandleRes.Success)
                {
                    Channel.BasicAck(e.DeliveryTag, false); //确认消息已处理
                }
                else
                {
                    Channel.BasicReject(e.DeliveryTag, false); //失败
                }
                if (handleRes != MsgHandleRes.FailAndReQueue) return;

                #region ReDelivery 延迟或死信

                const string keyReDelivery = Utility.ProdBrand + "_redelivery";
                var prop = e.BasicProperties;
                object redObj = null;
                var reDelivery = prop.Headers?.TryGetValue(keyReDelivery, out redObj) == true ? (int) redObj : 0;

                //超限则丢入死信以作记录
                if (reDelivery >= MaxReDelivery)
                {
                    Channel.BasicPublish(MqBroker.ExchangeCommonDl, _consumeQueue, true, prop, e.Body);
                    LogService.ErrorFormat("MQ消息处理失败，且已达最大重试次数{0}。“{1}”", reDelivery, msgData);
                    return;
                }

                //发送到延迟队列，依靠超时实现的延迟。 https://www.rabbitmq.com/dlx.html
                if (prop.Headers == null) prop.Headers = new Dictionary<string, object>();
                else prop.Headers.Remove("x-death"); //移除每次从延迟队列出来的标记，否则会累积起来。
                
                prop.Headers[keyReDelivery] = ++reDelivery;
                Channel.BasicPublish(MqBroker.ExchangeDelay10Min, _consumeQueue, true, prop, e.Body);

                #endregion
            }
            catch (Exception ex)
            {
                LogService.Error(ex);
            }

            #endregion
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            try
            {
                if (Channel == null) return;
                if (_innerConsumer?.ConsumerTags != null)
                {
                    foreach (var tag in _innerConsumer.ConsumerTags)
                    {
                        Channel.BasicCancel(tag);
                    }
                }

                Channel.Close();
                Channel.Dispose();
            }
            catch (Exception e)
            {
                LogService.Error(e);
            }
        }

        #endregion
    }

    public enum MsgHandleRes
    {
        Success = 0,
        
        /// <summary>
        /// 失败，将延迟（默认10分钟）后重新投递进队列，默认重试5次
        /// </summary>
        FailAndReQueue = 1,

        /// <summary>
        /// 失败，消息会被丢弃
        /// </summary>
        FailAndReject = 2
    }
}
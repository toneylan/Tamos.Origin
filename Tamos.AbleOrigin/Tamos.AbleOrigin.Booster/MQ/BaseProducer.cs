using System;
using System.Text;
using RabbitMQ.Client;
using Tamos.AbleOrigin.Log;
using Tamos.AbleOrigin.Serialize;

namespace Tamos.AbleOrigin.Booster
{
    /// <summary>
    /// 消息生产者基类
    /// </summary>
    public class BaseProducer : IDisposable
    {
        internal IModel Channel { get; private set; }
        private IBasicProperties _commonProp;

        internal void Init(IModel model)
        {
            Channel = model;
            Channel.BasicReturn += Channel_BasicReturn;

            _commonProp = Channel.CreateBasicProperties();
            _commonProp.ContentType = "application/json";
            _commonProp.DeliveryMode = 2; //persistent msg
        }

        #region Publish

        /// <summary>
        /// exchange为string.Empty时，routingKey就是要发布到的Queue。
        /// </summary>
        public virtual void Publish(string exchange, string routingKey, object content)
        {
            if (Channel == null) return;

            try
            {
                var jsonStr = SerializeUtil.ToJson(content);

                //MQ 打印日志
                LogService.Debug($"{exchange}[{routingKey}]: {jsonStr}");

                var bodyData = string.IsNullOrEmpty(jsonStr) ? null : Encoding.UTF8.GetBytes(jsonStr);

                lock (Channel)
                {
                    _commonProp?.ClearMessageId();
                    Channel.BasicPublish(exchange, routingKey, true, _commonProp, bodyData);
                }
            }
            catch (Exception e)
            {
                LogService.Error(e);
            }
        }

        #endregion

        #region Event Handle、Dispose
        
        private void Channel_BasicReturn(object sender, RabbitMQ.Client.Events.BasicReturnEventArgs e)
        {
            LogService.ErrorFormat("MQ BasicReturn(PublishFail) Exchange:{0}, RoutingKey:{1}, Reply:{2}-{3}",
                e.Exchange, e.RoutingKey, e.ReplyCode, e.ReplyText);
        }

        public virtual void Dispose()
        {
            try
            {
                Channel?.Close();
                Channel?.Dispose();
            }
            catch (Exception e)
            {
                LogService.Error(e);
            }
        }

        #endregion
    }
}
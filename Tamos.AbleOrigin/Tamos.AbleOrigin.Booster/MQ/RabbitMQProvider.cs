using System;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace Tamos.AbleOrigin.Booster
{
    /// <summary>
    /// MQ服务的功能代理(Singleton)
    /// </summary>
    internal class RabbitMQProvider : IMQProvider, IDisposable
    {
        //默认连接，相当于一个Tcp连接，在进程级别供多路Channel复用。
        //若在高并发场景，考虑改进为多个Connection、池化等方案，来避免单连接的性能瓶颈。
        private readonly IConnection Connection = CreateConnection();

        private List<MQProducer>? _producers; //记录创建的Producers
        private List<IDisposable>? _consumers; //记录创建的Consumers
        
        private MQProducer? _defaultProducer;

        /// <summary>
        /// 默认Producer，可用于快捷的发布消息
        /// </summary>
        private MQProducer DefaultProducer => _defaultProducer ??= (MQProducer)CreateProducer();

        #region CreateConnection

        private static IConnection CreateConnection()
        {
            try
            {
                //中途的网络连接异常后可以自动恢复
                var factory = new ConnectionFactory
                {
                    HostName = ServiceAddressConfig.GetExternalSvcSet("mq_host", "host.docker.internal"),
                    UserName = ServiceAddressConfig.GetExternalSvcSet("mq_username", "guest"),
                    Password = ServiceAddressConfig.GetExternalSvcSet("mq_password", "guest")
                };

                //create connect
                return factory.CreateConnection();
            }
            catch (Exception e)
            {
                LogService.ErrorFormat("连接MQ失败：{0}", e);
                return null!;
            }
        }

        #endregion
        
        #region Declare schema
        
        //定义基础组件
        void IMQProvider.DeclareCommonInfra()
        {
            DefaultProducer.DeclareQueue(MQConfig.CommonDl);
            
            //延迟Schema
            var channel = DefaultProducer.Channel;
            channel.ExchangeDeclare(MQConfig.Delay10Min, ExchangeType.Topic, true);
            channel.QueueDeclare(MQConfig.Delay10Min, true, false, false, new Dictionary<string, object>
            {
                {"x-dead-letter-exchange", string.Empty}, //Empty代表direct exchange, 入队时的routing key作为dead-letter-routing-key
                {"x-message-ttl", 10 * 60000} //过期分钟（即延迟）
            });
            channel.QueueBind(MQConfig.Delay10Min, MQConfig.Delay10Min, "#"); //"#"通配零个或多个单词。
        }

        
        void IMQProvider.DeclareSchema(Action<IMQProducer> declareAction)
        {
            declareAction.Invoke(DefaultProducer);
        }

        #endregion

        #region Create Producer Consumer

        public IMQProducer CreateProducer(bool logMessage = false)
        {
            var producer = new MQProducer(Connection.CreateModel());
            producer.LogMessage = logMessage;

            (_producers ??= new List<MQProducer>()).Add(producer);
            return producer;
        }

        public IMQConsumer CreateConsumer<T>(string queueName, Func<T, MQHandleRes> handler)
        {
            var consumer = new MQConsumer<T>(Connection.CreateModel(), queueName)
            {
                MessageHandler = handler
            };

            (_consumers ??= new ()).Add(consumer);
            return consumer;
        }

        #endregion


        #region Impl IMQProvider

        public void Publish<T>(string queueName, T content)
        {
            DefaultProducer.Publish(string.Empty, queueName, content);
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            try
            {
                _producers?.ForEach(x => x.Dispose());
                _consumers?.ForEach(x => x.Dispose());

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (Connection != null)
                {
                    Connection.Close();
                    Connection.Dispose();
                }
            }
            catch (Exception e)
            {
                LogService.Error(e);
            }
        }

        #endregion    
    }
}
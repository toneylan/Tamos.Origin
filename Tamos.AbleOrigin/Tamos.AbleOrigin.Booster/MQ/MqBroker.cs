using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using Tamos.AbleOrigin.Log;

namespace Tamos.AbleOrigin.Booster
{
    /// <summary>
    /// 一个MQ服务的功能代理
    /// </summary>
    public class MqBroker : IDisposable
    {
        #region Creator

        //常量名称
        public static readonly string ExchangeDelay10Min = "bonny_exch_base_delay_10min";
        public static readonly string ExchangeCommonDl = "bonny_exch_base_common_dl";

        public static MqBroker ConnectTo(string userName, string password, string host)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    UserName = userName,
                    Password = password,
                    HostName = host,
                    AutomaticRecoveryEnabled = true //启用连接异常后的自动恢复
                };

                //create broker and connect
                var broker = new MqBroker
                {
                    _connection = factory.CreateConnection()
                };
                return broker;
            }
            catch (Exception e)
            {
                LogService.ErrorFormat("连接MQ({0})失败：{1}", host, e);
                return null;
            }
        }

        #endregion

        private IConnection _connection;
        private List<BaseProducer> _producers; //记录创建的Producers
        private List<BaseConsumer> _consumers; //记录创建的Consumers
        private BaseProducer _defaultProducer;

        /// <summary>
        /// 默认Producer，可用于快捷的发布消息
        /// </summary>
        public BaseProducer DefaultProducer => _defaultProducer ??= CreateProducer<BaseProducer>();

        #region Declare

        /// <summary>
        /// Exchange、Queue定义工作
        /// </summary>
        public void DeclareWork(Action<IModel> declareAction)
        {
            using var model = _connection.CreateModel();
            try
            {
                declareAction.Invoke(model);
            }
            catch (Exception e)
            {
                LogService.Error(e);
            }
        }

        #endregion

        #region Producer Consumer

        public T CreateProducer<T>() where T : BaseProducer, new()
        {
            var producer = new T();
            producer.Init(_connection.CreateModel());

            (_producers ??= new List<BaseProducer>()).Add(producer);
            return producer;
        }

        public T CreateConsumer<T>(string queue, Func<string, MsgHandleRes> handler) where T : BaseConsumer, new ()
        {
            var consumer = new T
            {
                MessageHandler = handler
            };
            consumer.Init(_connection.CreateModel(), queue);

            (_consumers ??= new List<BaseConsumer>()).Add(consumer);
            return consumer;
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            try
            {
                _producers?.ForEach(x => x.Dispose());
                _consumers?.ForEach(x => x.Dispose());

                if (_connection != null)
                {
                    _connection.Close();
                    _connection.Dispose();
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
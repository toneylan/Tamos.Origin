using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Tamos.AbleOrigin
{
    /// <summary>
    /// 分布式系统的基础功能服务，依靠Redis实现
    /// </summary>
    public static class DistributedService
    {
        /// <summary>
        /// Redis实现Provider
        /// </summary>
        public static readonly IDistributedSrvProvider Provider = ServiceLocator.GetOrReflect<IDistributedSrvProvider>("RedisDistSrvProvider")!;

        #region Lock

        /// <summary>
        /// 开启分布式锁。
        /// </summary>
        /// <param name="name">锁名称</param>
        /// <param name="lockTimeout">锁在多久后，即使未释放也会自动超时释放</param>
        /// <param name="waitTimeout">等待获取锁的超时时间</param>
        /// <param name="resLock">获得的锁实例</param>
        /// <returns>是否获取锁成功</returns>
        public static bool Lock(string name, TimeSpan lockTimeout, TimeSpan waitTimeout, [NotNullWhen(true)] out IDisposable? resLock)
        {
            //设置锁
            name = $"Lock:{name}";
            var lockVal = Guid.NewGuid().ToString();
            var startTime = DateTime.Now;
            var hasLock = false;
            do
            {
                if (Provider.Lock(name, lockVal, lockTimeout))
                {
                    hasLock = true;
                    break;
                }

                if (waitTimeout.TotalMilliseconds > 0) Thread.Sleep(1000);
            } while (DateTime.Now.Subtract(startTime) <= waitTimeout);

            resLock = hasLock ? new DistributedLock {Name = name, Value = lockVal} : null;
            return hasLock;
        }

        /*/// <summary>
        /// 释放分布式锁
        /// </summary>
        public static bool Unlock(string name, string lockVal)
        {
            return Provider.Unlock(name, lockVal);
        }*/

        /// <summary>
        /// 开启分布式锁，以执行过程。注意：方法返回时锁已经释放了。
        /// </summary>
        /// <param name="name">锁名称</param>
        /// <param name="lockTimeout">锁在多久后，即使未释放也会自动超时释放</param>
        /// <param name="waitTimeout">等待获取锁的超时时间</param>
        /// <param name="action">获取锁成功后执行的过程</param>
        /// <returns>是否成功获取锁来执行action</returns>
        public static bool RunInLock(string name, TimeSpan lockTimeout, TimeSpan waitTimeout, Action action)
        {
            if (!Lock(name, lockTimeout, waitTimeout, out var resLock)) return false;

            //执行过程
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                LogService.Error(e);
            }
            finally
            {
                resLock.Dispose();
            }
            return true;
        }

        #endregion

        #region Pub/Sub

        //分布式事件服务（Redis pub/sub 模式来实现），一般用于事件驱动的业务过程，对消息存储不会太严格，否则应使用MQ。

        /*/// <summary>
        /// 发布事件消息。用topic区分类别。
        /// </summary>
        public static void Publish<T>(string topic, T eventMsg)
        {
            Provider.Publish(topic, eventMsg);
        }

        /// <summary>
        /// 订阅指定topic的事件消息。
        /// </summary>
        public static IDisposable Subscribe<T>(string topic, Action<T> handler)
        {
            return Provider.Subscribe(topic, handler);
        }*/

        #endregion
    }
}
using System;

namespace Tamos.AbleOrigin.DataPersist
{
    /*/// <summary>
    /// 实现DbContext的创建及缓存。
    /// </summary>
    internal class DbCreater<T> : IDbCreater<T> where T : BaseDbContext, new()
    {
        private T? _dbInstance; //直接保存一个Db实例
        
        /// <summary>
        /// 获取或新建实例
        /// </summary>
        public T GetDb()
        {
            return _dbInstance ??= new T();
        }

        public void Dispose()
        {
            _dbInstance?.Dispose();
        }
    }

    /// <summary>
    /// 常规DbContext的Creater。
    /// </summary>
    internal interface IDbCreater<out T> : IDisposable where T : BaseDbContext
    {
        /// <summary>
        /// 获取Db实例，会缓存于Creater实例中。
        /// </summary>
        T GetDb();
    }*/
}
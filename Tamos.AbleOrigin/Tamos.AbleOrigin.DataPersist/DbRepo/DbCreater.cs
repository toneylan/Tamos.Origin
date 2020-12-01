namespace Tamos.AbleOrigin.DataPersist
{
    /// <summary>
    /// 实现DbContext的创建及缓存。
    /// </summary>
    internal class DbCreater<T> : IDbCreater<T> where T : BaseDbContext, new()
    {
        private T _dbInstance; //直接保存一个Db实例

        //private readonly BaseServiceComponent _srvComponent; //利用来缓存Db
        /*public DbCreater(service)
        {
            _srvComponent = service;
        }*/

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
}
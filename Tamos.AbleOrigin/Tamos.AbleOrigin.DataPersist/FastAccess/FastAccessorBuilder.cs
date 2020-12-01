using Tamos.AbleOrigin.DataProto;
using Tamos.AbleOrigin.ServiceBase;

namespace Tamos.AbleOrigin.DataPersist
{
    public class FastAccessorBuilder<TDb> where TDb : BaseDbContext, new()
    {
        /// <summary>
        /// 获取访问器（会缓存于service实例）
        /// </summary>
        public FastAccessor<TDTO, TEntity> Get<TDTO, TEntity>(BaseServiceComponent service)
            where TDTO : IGeneralEntity where TEntity : class, IGeneralEntity
        {
            return service.GetComponent(() => new FastAccessor<TDTO, TEntity>(service.GetComponent<DbCreater<TDb>>()));
        }
    }

    public class ShardAccessorBuilder<TDb> where TDb : ShardingDbContext
    {
        public ShardAccessorBuilder()
        {
            ShardingDbContext.EnsureDbTypeReg(typeof(TDb));
        }
        
        /// <summary>
        /// 获取访问器（会缓存于service实例）
        /// </summary>
        public FastShardAccessor<TDTO, TEntity> Get<TDTO, TEntity>(BaseServiceComponent service)
            where TDTO : IGeneralEntity where TEntity : class, IGeneralEntity
        {
            return service.GetComponent(() => new FastShardAccessor<TDTO, TEntity>(service.GetComponent<ShardingDbCreater<TDb>>()));
        }
    }
}
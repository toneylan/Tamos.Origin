using System;
using System.Threading.Tasks;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.DataPersist
{
    public class AccessorBuilder<TDb> where TDb : BaseDbContext, new()
    {
        protected virtual DbAccessor GetDbAccessor(ServiceComponent service)
        {
            return service.GetComponent<DefaultDbAccessor<TDb>>();
        }

        /// <summary>
        /// 获取访问器（会缓存于service实例）
        /// </summary>
        public FastAccessor<TDTO, TEntity> Get<TDTO, TEntity>(ServiceComponent service)
            where TDTO : IGeneralEntity where TEntity : class, IGeneralEntity
        {
            return service.GetComponent(() => new FastAccessor<TDTO, TEntity>(GetDbAccessor(service)));
        }

        #region Transaction

        public string? BeginTransaction(ServiceComponent service, Func<DbAccessor, string?> tranWork)
        {
            var dbAcs = GetDbAccessor(service);
            using var tran = dbAcs.CurDb.BeginTransaction();

            var error = tranWork(dbAcs);
            return CompleteTran(tran, error);
        }

        /// <summary>
        /// 执行异步事务
        /// </summary>
        public async Task<string?> BeginTranAsync(ServiceComponent service, Func<DbAccessor, Task<string?>> tranWork)
        {
            var dbAcs = GetDbAccessor(service);
            using var tran = dbAcs.CurDb.BeginTransaction();
            
            var error = await tranWork(dbAcs);
            return CompleteTran(tran, error);
        }

        private static string? CompleteTran(DbTransactionSet tran, string? workErr)
        {
            try
            {
                if (workErr.NotNull())
                {
                    tran.Rollback();
                    return workErr;
                }

                tran.Commit();
                return null;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                LogService.Error(ex);
                return ex.Message;
            }
        }

        #endregion
    }

    public class ShardAccessorBuilder<TDb> : AccessorBuilder<TDb> where TDb : ShardingDbContext, new()
    {
        protected override DbAccessor GetDbAccessor(ServiceComponent service)
        {
            var db = service.GetComponent<ShardingDbCreater<TDb>>().By(DateTime.Today);
            return new DefaultDbAccessor<TDb>(db);
        }

        /// <summary>
        /// 获取访问器（会缓存于service实例）
        /// </summary>
        public FastShardAccessor<TDTO, TEntity> GetShard<TDTO, TEntity>(ServiceComponent service)
            where TDTO : IGeneralEntity where TEntity : class, IGeneralEntity
        {
            return service.GetComponent(() => new FastShardAccessor<TDTO, TEntity>(service.GetComponent<ShardingDbCreater<TDb>>()));
        }
    }
}
using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Tamos.AbleOrigin.DataPersist
{
    public abstract class BaseDbContext : DbContext
    {
        #region Static Mysql use

        protected static DbContextOptions GetDbOpts(string connectionString)
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseMySql(connectionString);
            return builder.Options;
        }

        protected static DbContextOptions GetDbOpts(DbConnection connection)
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseMySql(connection);
            return builder.Options;
        }

        /// <summary>
        /// 打开新的DbContext来执行Sql语句
        /// </summary>
        internal static void RunSql(string connectionString, string sql)
        {
            new DbContext(GetDbOpts(connectionString)).Database.ExecuteSqlRaw(sql);
        }

        #endregion

        #region Ctor

        /*protected BaseDbContext()
        {
        }*/

        protected BaseDbContext(string connectionString) : base(GetDbOpts(connectionString))
        {
        }

        protected BaseDbContext(DbConnection connection) : base(GetDbOpts(connection))
        {
        }

        /// <summary>
        /// ShardingDb不能直接调用，构造时Scope为空，DynamicModelCacheKeyFactory会报错。
        /// </summary>
        protected BaseDbContext(DbTransactionContext dbTran) : base(GetDbOpts(dbTran.Connection))
        {
            UseTransaction(dbTran);
        }

        #endregion

        #region Transaction

        public DbTransactionContext BeginTransaction()
        {
            return new DbTransactionContext(Database);
        }

        internal void UseTransaction(DbTransactionContext tran)
        {
            Database.UseTransaction(tran.Transaction.GetDbTransaction());
        }

        #endregion

        #region EntityDbMap

        protected internal EntityDbMap GetDbMap<T>()
        {
            return EntityDbMap.GetOrAdd<T>(() =>
            {
                var entityType = Model.FindEntityType(typeof(T));
                if (entityType == null) throw new Exception($"从{GetType().Name}获取{typeof(T)}的配置失败");
                return entityType;
            });
        }

        #endregion

        /*#region Attach

        /// <summary>
        /// 附加对象到DbContext，以进行修改设置
        /// </summary>
        public void SafeAttach<T>(T entity, EntityState? state = null) where T : class
        {
            var entry = Entry(entity);
            if (state != null)
            {
                entry.State = state.Value;
            }
            else if (entry.State == EntityState.Detached)
            {
                entry.State = EntityState.Unchanged; //等于Attach操作
            }
        }

        #endregion*/
    }
}
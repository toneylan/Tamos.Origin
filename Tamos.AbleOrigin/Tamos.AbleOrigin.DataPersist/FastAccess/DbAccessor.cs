using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.DataPersist
{
    public abstract class DbAccessor : IDisposable
    {
        //protected bool InUnitWork { get; init; }

        internal abstract BaseDbContext CurDb { get; }

        #region Add

        /// <summary>
        /// Add record, without save.
        /// </summary>
        public void Add<T>(T po) where T : class
        {
            CurDb.Set<T>().Add(po);
        }

        /// <summary>
        /// Add record list, without save.
        /// </summary>
        public void AddRange<T>(IEnumerable<T> items) where T : class
        {
            CurDb.Set<T>().AddRange(items);
        }

        #endregion

        #region Update
        
        /// <summary>
        /// Update record by Id, without save.
        /// </summary>
        public bool Update<T>(long id, Action<T> setDbItem) where T : class, IGeneralEntity
        {
            var dbItem = CurDb.Set<T>().FirstOrDefault(x => x.Id == id);
            if (dbItem == null) return false;

            setDbItem(dbItem);
            return true;
        }

        /// <summary>
        /// 执行Sql更新语句，按Id及其他条件，只修改一行。<br/>
        /// WherePart不用设置Id条件。
        /// </summary>
        public bool UpdateBySql<T>(long id, Func<EntityDbMap, (string SetPart, string? WherePart)> buildSql)
        {
            var map = CurDb.GetMapPO<T>();
            const string idName = nameof(IGeneralEntity.Id);

            var (setPart, wherePart) = buildSql(map);
            if (setPart.IsNull()) return false;

            var sql = $"UPDATE {map.TableName} SET {setPart} WHERE {map.Col(idName)}={id}{wherePart.Append(" AND ")} LIMIT 1;";
            return CurDb.Database.ExecuteSqlRaw(sql) > 0;
        }

        #endregion

        /// <summary>
        /// 执行DbContext.SaveChanges，保存前边的数据操作（UnitOfWork）。
        /// </summary>
        public int SaveChanges()
        {
            return CurDb.SaveChanges();
        }

        public abstract void Dispose();
    }

    internal class DefaultDbAccessor<T> : DbAccessor where T : BaseDbContext, new()
    {
        //private readonly DbCreater<T> DbCreater = new(); //统一管理DbContext（Factory模式）
        private readonly T _dbInstance; //直接保存一个Db实例
        private readonly bool _disposeDb;

        internal override BaseDbContext CurDb => _dbInstance;

        public DefaultDbAccessor()
        {
            _dbInstance = new T();
            _disposeDb = true;
        }

        internal DefaultDbAccessor(T instance, bool disposeDb = false)
        {
            _dbInstance = instance;
            _disposeDb = disposeDb;
        }

        public override void Dispose()
        {
            if (_disposeDb) _dbInstance.Dispose();
        }
    }

    /*internal class UnitDbAccessor : BaseDbAccessor
    {
        private readonly BaseDbContext _db;

        protected override BaseDbContext GetDb() => _db;

        internal UnitDbAccessor(BaseDbContext db)
        {
            _db = db;
            InUnitWork = true;
        }
    }*/
}
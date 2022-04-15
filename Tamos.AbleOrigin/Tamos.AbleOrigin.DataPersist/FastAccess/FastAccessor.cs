using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.DataPersist
{
    /// <summary>
    /// 实现Id为主键的数据，Db记录的快速访问器。<br/>
    /// 轻量级创建，如一个BaseServiceComponent类中，可同时为多个表创建Accessor，DbContext被缓存与共享。
    /// </summary>
    public class FastAccessor<TDTO, TEntity> : BaseFastAccessor, IFastAccessor<TDTO>
        where TDTO : IGeneralEntity where TEntity : class, IGeneralEntity
    {
        private readonly DbAccessor DbAcs;

        internal FastAccessor(DbAccessor dbAcs)
        {
            DbAcs = dbAcs;
        }

        #region 未分表 Db and DbSet

        /// <summary>
        /// 获取Db实例
        /// </summary>
        protected internal BaseDbContext Db => DbAcs.CurDb;

        /// <summary>
        /// 获取IQueryable实例AsNoTracking
        /// </summary>
        public IQueryable<TEntity> Queryable()
        {
            return Db.Set<TEntity>().AsNoTracking();
        }

        /// <summary>
        /// 获取IQueryable实例AsNoTracking
        /// </summary>
        public IQueryable<TEntity> Queryable(Expression<Func<TEntity, bool>> predicate)
        {
            return Db.Set<TEntity>().AsNoTracking().Where(predicate);
        }

        /*//返回Entity Set
        private static DbSet<TEntity> DbSet(BaseDbContext db) => db.Set<TEntity>();*/

        #endregion

        #region Query

        /// <summary>
        /// 依据Id获取单个记录
        /// </summary>
        public virtual TDTO? Get(long id)
        {
            return Queryable().FirstOrDefault(x => x.Id == id).MapTo<TDTO>();
        }

        /// <summary>
        /// 依据Id列表获取
        /// </summary>
        public virtual List<TDTO>? Get(IReadOnlyCollection<long> ids)
        {
            return ids.IsNull() ? null : Queryable(x => ids.Contains(x.Id)).ToList().MapTo<List<TDTO>>();
        }

        /// <summary>
        /// 按条件查询首条记录
        /// </summary>
        public virtual TDTO? FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Queryable().FirstOrDefault(predicate).MapTo<TDTO>();
        }

        /// <summary>
        /// 按条件查询列表
        /// </summary>
        public virtual List<TDTO>? QueryList(Expression<Func<TEntity, bool>> predicate)
        {
            return Queryable(predicate).ToList().MapTo<List<TDTO>>();
        }

        #endregion

        #region Write

        /// <summary>
        /// 添加记录
        /// </summary>
        public bool Add(TDTO dto)
        {
            return Add(dto.MapTo<TEntity>());
        }

        /// <summary>
        /// 添加记录
        /// </summary>
        public bool Add(TEntity entity)
        {
            var db = Db;
            db.Set<TEntity>().Add(entity);
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// 添加多条记录
        /// </summary>
        public bool AddRange(IEnumerable<TEntity> entitys)
        {
            var db = Db;
            db.Set<TEntity>().AddRange(entitys);
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// 按Id删除记录
        /// </summary>
        public bool Delete(long id)
        {
            return DoDelete<TEntity>(Db, id);
        }

        /// <summary>
        /// 按Id删除记录，成功后执行回调
        /// </summary>
        public bool Delete(long id, Action<TEntity> afterDel)
        {
            return DoDelete(Db, id, afterDel);
        }

        /// <summary>
        /// 按Id删除记录
        /// </summary>
        public bool Delete(List<long> ids)
        {
            if (ids.IsNull()) return false;

            return DoDelete<TEntity>(Db, ids);
        }

        /// <summary>
        /// Delete records by condition, Be carefully filter the correct record range.<br/>
        /// Out of the maxCount, will throw error.
        /// </summary>
        public bool DeleteBy(Expression<Func<TEntity, bool>> predicate, int maxCount = 100)
        {
            var db = Db;
            var set = db.Set<TEntity>();
            var delItems = set.Where(predicate).Take(maxCount + 1).ToList();
            if (delItems.Count == 0) return false;
            if (delItems.Count > maxCount) throw new Exception("数据删除异常，超出了预计的删除数量。");

            set.RemoveRange(delItems);
            return db.SaveChanges() > 0;
        }

        #endregion

        #region Update

        /// <summary>
        /// Update db record by id, has call db.SaveChanges.
        /// </summary>
        public bool Update(long id, Action<TEntity> setDbItem)
        {
            if (!DbAcs.Update(id, setDbItem)) return false;
            DbAcs.SaveChanges(); //如果属性没变化，不会触发写库。
            return true;
        }

        /// <summary>
        /// 更新多条记录，无论是否查询到记录，都会调用setDbItems
        /// </summary>
        public void Update(IReadOnlyList<long> ids, Action<List<TEntity>> setDbItems)
        {
            if (ids.IsNull()) return;

            var db = Db;
            var dbItems = db.Set<TEntity>().Where(x => ids.Contains(x.Id)).ToList();
            setDbItems(dbItems);

            db.SaveChanges();
        }

        /// <summary>
        /// Query db items by id, and do update by custom action.
        /// </summary>
        public int Update<T>(IReadOnlyCollection<T> items, Action<T, TEntity> updateAction) where T : IGeneralEntity
        {
            if (items.Count == 0) return 0;

            var ids = items.Select(x => x.Id).ToList();
            var db = Db;
            var dbItems = db.Set<TEntity>().Where(x => ids.Contains(x.Id)).ToList();

            foreach (var dbItem in dbItems)
            {
                var toItem = items.First(x => x.Id == dbItem.Id);
                updateAction(toItem, dbItem);
            }

            return db.SaveChanges();
        }

        /// <summary>
        /// Custom query and update data, has call db.SaveChanges.
        /// </summary>
        public void UpdateBy(Action<IQueryable<TEntity>> qryAndSet)
        {
            var db = Db;
            qryAndSet(db.Set<TEntity>());

            db.SaveChanges();
        }

        /// <summary>
        /// 执行Sql语句来更新，只修改等于Id的一行。
        /// </summary>
        public bool UpdateBySql(long id, Func<EntityDbMap, (string SetPart, string? WherePart)> buildSql)
        {
            return DbAcs.UpdateBySql<TEntity>(id, buildSql);
        }

        #endregion
    }
}
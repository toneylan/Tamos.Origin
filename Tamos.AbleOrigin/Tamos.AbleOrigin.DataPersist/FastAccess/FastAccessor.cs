using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Tamos.AbleOrigin.Common;
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
        private readonly IDbCreater<BaseDbContext> DbCreater;

        internal FastAccessor(IDbCreater<BaseDbContext> dbCreater)
        {
            DbCreater = dbCreater;
        }

        #region 未分表 Db and DbSet

        /// <summary>
        /// 获取Db实例
        /// </summary>
        protected internal BaseDbContext Db => DbCreater.GetDb();

        /// <summary>
        /// 获取IQueryable实例AsNoTracking
        /// </summary>
        public IQueryable<TEntity> Queryable()
        {
            return Db.Set<TEntity>().AsNoTracking(); //return tracking ? DbSet : DbSet.AsNoTracking();
        }

        /*//返回Entity Set
        private static DbSet<TEntity> DbSet(BaseDbContext db) => db.Set<TEntity>();*/

        #endregion

        #region Query

        /// <summary>
        /// 依据Id获取单个记录
        /// </summary>
        public virtual TDTO Get(long id)
        {
            return Queryable().FirstOrDefault(x => x.Id == id).MapTo<TDTO>();
        }

        /// <summary>
        /// 依据Id列表获取
        /// </summary>
        public virtual List<TDTO> Get(IReadOnlyCollection<long> ids)
        {
            return ids.IsNullOrEmpty() ? null : Queryable().Where(x => ids.Contains(x.Id)).ToList().MapTo<List<TDTO>>();
        }

        /// <summary>
        /// 按条件查询首条记录
        /// </summary>
        public virtual TDTO FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Queryable().FirstOrDefault(predicate).MapTo<TDTO>();
        }

        /// <summary>
        /// 按条件查询列表
        /// </summary>
        public virtual List<TDTO> QueryList(Expression<Func<TEntity, bool>> predicate)
        {
            var query = Queryable().Where(predicate);
            return query.ToList().MapTo<List<TDTO>>();
        }

        #endregion

        #region Write

        /// <summary>
        /// 添加记录
        /// </summary>
        public virtual bool Add(TDTO dto)
        {
            return Add(dto.MapTo<TEntity>());
        }

        /// <summary>
        /// 添加记录
        /// </summary>
        public virtual bool Add(TEntity entity)
        {
            var db = Db;
            db.Set<TEntity>().Add(entity);
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// 更新存储的记录
        /// </summary>
        public virtual bool Update(long id, Action<TEntity> setDbItem)
        {
            return DoUpdate(Db, id, setDbItem);
        }

        /// <summary>
        /// 按Id删除记录
        /// </summary>
        public virtual bool Delete(long id)
        {
            return DoDelete<TEntity>(Db, id);
        }

        /// <summary>
        /// 按Id删除记录，成功后执行回调
        /// </summary>
        public bool Delete(long id, Action<TEntity> afterDel)
        {
            return DoDelete<TEntity>(Db, id, afterDel);
        }

        /// <summary>
        /// 按Id删除记录
        /// </summary>
        public virtual bool Delete(List<long> ids)
        {
            if (ids.IsNullOrEmpty()) return false;

            return DoDelete<TEntity>(Db, ids);
        }

        /// <summary>
        /// 执行Sql语句来更新，只修改等于Id的一行。
        /// </summary>
        public bool UpdateBySql(long id, Func<EntityDbMap, string> funSetPart)
        {
            var db = Db;
            var map = db.GetDbMap<TEntity>();
            const string propId = nameof(IGeneralEntity.Id);
            
            var sql = $"UPDATE {map.TableName} SET {funSetPart(map)} WHERE {map.Col(propId)}={id} LIMIT 1;";
            return db.Database.ExecuteSqlRaw(sql) > 0;
        }

        #endregion
    }
}
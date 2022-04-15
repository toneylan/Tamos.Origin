using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.DataPersist
{
    /// <summary>
    /// 分表访问器，Id为主键且用于解析时间。
    /// </summary>
    public class FastShardAccessor<TDTO, TEntity> : BaseFastAccessor, IFastAccessor<TDTO>
        where TDTO : IGeneralEntity where TEntity : class, IGeneralEntity
    {
        private readonly IShardingDbCreater<ShardingDbContext> DbCreater;

        internal FastShardAccessor(IShardingDbCreater<ShardingDbContext> dbCreater)
        {
            DbCreater = dbCreater;
        }

        #region 分表 Db

        /// <summary>
        /// 获取IQueryable实例AsNoTracking
        /// </summary>
        protected IQueryable<TEntity> Queryable(ShardingDbContext db)
        {
            return db.Set<TEntity>().AsNoTracking();
        }

        #endregion

        #region Query

        /// <summary>
        /// 依据Id获取单个记录
        /// </summary>
        public TDTO? Get(long id)
        {
            return Queryable(DbCreater.By(id)).FirstOrDefault(x => x.Id == id).MapTo<TDTO>();
        }

        /// <summary>
        /// 按条件获取首条记录
        /// </summary>
        public TDTO? Get(DateTime time, Expression<Func<TEntity, bool>> predicate)
        {
            return Queryable(DbCreater.By(time)).FirstOrDefault(predicate).MapTo<TDTO>();
        }

        /// <summary>
        /// 依据Id列表获取
        /// </summary>
        public List<TDTO>? Get(IReadOnlyCollection<long> ids)
        {
            if (ids.IsNull()) return null;

            return DbCreater.By(ids).Query(db => Queryable(db).Where(x => ids.Contains(x.Id)))
                .ToList().MapTo<List<TDTO>>();
        }
        
        /// <summary>
        /// 按条件查询列表，时间仅用于定位分表
        /// </summary>
        public List<TDTO>? QueryList(DateTime start, DateTime end, Expression<Func<TEntity, bool>> predicate)
        {
            return DbCreater.By(start, end).Query(db => Queryable(db).Where(predicate))
                .ToList().MapTo<List<TDTO>>();
        }

        /// <summary>
        /// 按条件查询列表，时间仅用于定位分表
        /// </summary>
        public List<TDTO>? QueryList(DateTime start, DateTime end, Func<IQueryable<TEntity>, IQueryable<TEntity>> buildQuery)
        {
            return DbCreater.By(start, end).Query(db => buildQuery(Queryable(db)))
                .ToList().MapTo<List<TDTO>>();
        }

        #endregion

        #region Write

        /// <summary>
        /// 添加记录
        /// </summary>
        public bool Add(TEntity entity)
        {
            var db = DbCreater.By(entity.Id);
            db.Set<TEntity>().Add(entity);
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// 更新存储的记录
        /// </summary>
        public bool Update(long id, Action<TEntity> setDbItem)
        {
            return DoUpdate(DbCreater.By(id), id, setDbItem);
        }

        /// <summary>
        /// 按Id删除记录
        /// </summary>
        public bool Delete(long id)
        {
            return DoDelete<TEntity>(DbCreater.By(id), id);
        }

        #endregion
    }
}
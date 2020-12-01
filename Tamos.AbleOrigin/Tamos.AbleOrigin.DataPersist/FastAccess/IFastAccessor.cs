using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.DataPersist
{
    #region IFastAccessor

    /// <summary>
    /// 普通记录的快速访问器接口
    /// </summary>
    public interface IFastAccessor<TDTO> where TDTO : IGeneralEntity
    {
        /// <summary>
        /// 依据Id获取单个记录
        /// </summary>
        TDTO Get(long id);

        /// <summary>
        /// 依据Id列表获取
        /// </summary>
        List<TDTO> Get(IReadOnlyCollection<long> ids);

        /*/// <summary>
        /// 按条件查询首条记录
        /// </summary>
        TDTO FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 按条件查询列表
        /// </summary>
        List<TDTO> QueryList(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 添加记录
        /// </summary>
        void Add(TEntity entity);

        /// <summary>
        /// 按Id删除记录
        /// </summary>
        bool Delete(long id);

        /// <summary>
        /// 更新存储的记录
        /// </summary>
        bool Update(long id, Action<TEntity> setDbItem);*/
    }

    #endregion

    #region IFastShardAccessor

    /*public interface IFastShardAccessor<TDTO, TEntity> where TDTO : IGeneralEntity where TEntity : class, IGeneralEntity
    {
        /// <summary>
        /// 依据Id获取单个记录
        /// </summary>
        TDTO Get(long id);

        /// <summary>
        /// 依据Id列表获取
        /// </summary>
        List<TDTO> Get(List<long> ids);

        /// <summary>
        /// 按条件查询列表，时间仅用于定位分表
        /// </summary>
        List<TDTO> QueryList(DateTime start, DateTime end, Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 添加记录
        /// </summary>
        void Add(TEntity entity);

        /// <summary>
        /// 按Id删除记录
        /// </summary>
        bool Delete(long id);

        /// <summary>
        /// 更新存储的记录
        /// </summary>
        bool Update(long id, Action<TEntity> setDbItem);
    }*/

    #endregion
}
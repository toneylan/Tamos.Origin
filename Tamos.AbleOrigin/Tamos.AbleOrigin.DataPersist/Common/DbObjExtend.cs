using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tamos.AbleOrigin.Common;
using Tamos.AbleOrigin.DataProto;
using Tamos.AbleOrigin.Mapper;

namespace Tamos.AbleOrigin.DataPersist
{
    public static class DbObjExtend
    {
        #region FastAccessor
        
        /// <summary>
        /// 按父级Id查询
        /// </summary>
        public static List<TDTO> GetByParent<TDTO, TEntity>(this FastAccessor<TDTO, TEntity> accessor, long parentId)
            where TDTO : IGeneralEntity where TEntity : class, IGeneralSubEntity
        {
            return accessor.QueryList(x => x.ParentId == parentId);
        }

        /// <summary>
        /// 将子级数据一并查出
        /// </summary>
        public static TDTO GetWithSub<TDTO, TEntity, TSubDTO, TSubEntity>(this FastAccessor<TDTO, TEntity> accessor, long id, FastAccessor<TSubDTO, TSubEntity> subAcs)
            where TDTO : class, IGeneralEntity
            where TEntity : class, IGeneralParentEntity<TSubEntity>
            where TSubDTO : IGeneralEntity
            where TSubEntity : class, IGeneralSubEntity
        {
            var qry = from pt in accessor.Queryable()
                join sub in subAcs.Queryable() on pt.Id equals sub.ParentId into subs
                from item in subs.DefaultIfEmpty()
                where pt.Id == id
                select new JoinObj<TEntity, TSubEntity> {Data = pt, Item = item};

            return qry.ToList().CvtObj().MapTo<TDTO>();
        }

        /// <summary>
        /// 依据列表数据的关联Id，查询关联的记录并填充到相应的属性值。
        /// </summary>
        public static void FillProperty<TSource, TDTO>(this IReadOnlyCollection<TSource> list, Expression<Func<TSource, TDTO>> expressionProp, 
            Func<TSource, long> funcPropId, IFastAccessor<TDTO> accessor)
            where TDTO : IGeneralEntity
        {
            list.FillProperty(expressionProp, funcPropId, accessor.Get);

            /*if (list.IsNullOrEmpty()) return;
            
            var objList = accessor.Get(list.Select(funcPropId).ToList());
            if (objList.IsNullOrEmpty()) return;

            var setter = TypeExtend.GetPropertySetter(expressionProp);
            foreach (var item in list)
            {
                var obj = objList.ById(funcPropId(item));
                if (obj != null) setter(item, obj);
            }*/
        }

        #endregion

        #region JoinObj convert

        /// <summary>
        /// 注意！确保列表的TOuter是单个记录（Join造成的重复）。返回单个TOuter并设置TInner到SubItems。
        /// </summary>
        public static TOuter CvtObj<TOuter, TInner>(this IReadOnlyList<JoinObj<TOuter, TInner>> resList) where TOuter : class, IGeneralParentEntity<TInner>
        {
            if (resList.IsNullOrEmpty()) return null;

            var res = resList[0].Data;
            res.SubItems = new List<TInner>();
            foreach (var obj in resList)
            {
                if (obj.Item != null) res.SubItems.Add(obj.Item);
            }
            return res;
        }

        /// <summary>
        /// 对Id一对多Join查询结果，转换为GroupBy Id的列表，同时把Inner列表设置到Outer子项上。
        /// </summary>
        public static List<TOuter> CvtList<TOuter, TInner>(this IReadOnlyList<JoinObj<TOuter, TInner>> resList, Action<TOuter, List<TInner>> setItems) where TOuter : class, IGeneralEntity
        {
            if (resList.IsNullOrEmpty()) return null;
            
            return resList.GroupBy(x => x.Data.Id, (id, objList) =>
            {
                TOuter data = null;
                var items = new List<TInner>();
                foreach (var obj in objList)
                {
                    if (data == null) data = obj.Data;
                    items.Add(obj.Item);
                }
                setItems(data, items);
                return data;
            }).ToList();
        }

        #endregion

        /*#region Join Query

        public static void LeftJoinGroup<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer, IQueryable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Action<TOuter, List<TInner>> setItems)
            where TOuter : class
        {
            var qryJoin = outer.Join(inner, outerKeySelector, innerKeySelector, (left, right) => new JoinObj<TOuter, TInner> {Data = left, Item = right})
                ;
        }

        #endregion*/
            
        #region Sharding Query

        public static IEnumerable<TResult> Query<TDb, TResult>(this IReadOnlyList<TDb> dbs, Func<TDb, IQueryable<TResult>> query) where TDb : ShardingDbContext
        {
            if (dbs == null || dbs.Count == 0) return new List<TResult>();
            if (dbs.Count == 1) return query(dbs[0]);

            //在不同DbContext中，是无法进行Union all的(join 也是)
            //var resQry = dbs.Aggregate<TDb, IQueryable<TResult>>(null, (current, db) => current?.Concat(query(db)) ?? query(db));

            return dbs.SelectMany(query);
        }

        public static IEnumerable<TResult> PagingQuery<TDb, TResult>(this IReadOnlyList<TDb> dbs, Func<TDb, IQueryable<TResult>> query,
            int skip, int take) where TDb : ShardingDbContext
        {
            return DoPagingQuery(dbs, query, skip, take, false, out _);
        }

        public static IEnumerable<TResult> PagingQuery<TDb, TResult>(this IReadOnlyList<TDb> dbs, Func<TDb, IQueryable<TResult>> query,
            int skip, int take, out int count) where TDb : ShardingDbContext
        {
            return DoPagingQuery(dbs, query, skip, take, true, out count);
        }

        private static IEnumerable<TResult> DoPagingQuery<TDb, TResult>(IReadOnlyList<TDb> dbs, Func<TDb, IQueryable<TResult>> query,
            int skip, int take, bool withCount, out int count) where TDb : ShardingDbContext
        {
            count = 0;
            if (take >= 10000 && withCount) withCount = false; //查询大结果集时，不再计算总数

            if (dbs.Count < 1) return new List<TResult>();
            //单个分区时
            if (dbs.Count == 1)
            {
                var qr = query(dbs[0]);
                if (withCount) count = qr.Count();
                return skip == 0 ? qr.Take(take) : qr.Skip(skip).Take(take);
            }

            //跨分区查询
            var skipCount = 0; //前面分区已经Skip数量
            var resCount = 0;
            IEnumerable<TResult> res = null;
            foreach (var db in dbs)
            {
                var curQr = query(db);
                //统计总数
                var curAmount = 0; //当前分区总结果数
                if (withCount)
                {
                    curAmount = curQr.Count();
                    count += curAmount;
                }
                if (resCount >= take) continue; //已查完结果，继续以统计总数

                //当前结果集
                var curSkip = skip - skipCount;
                var curTake = take - resCount;
                var curRes = (curSkip <= 0 ? curQr.Take(curTake) : curQr.Skip(curSkip).Take(curTake)).ToList();
                resCount += curRes.Count;
                res = res?.Concat(curRes) ?? curRes;

                //是否查完结果
                if (resCount >= take)
                {
                    if (withCount) continue;
                    break;
                }
                //计算skipCount
                if (curSkip <= 0) continue;
                if (curRes.Count > 0) skipCount += curSkip; //有结果则肯定执行了skip
                else //无结果则skip了当前分区结果数
                {
                    if (!withCount) curAmount = curQr.Count();
                    skipCount += curAmount;
                }
            }

            return res;
        }

        /// <summary>
        /// 按分页参数查询
        /// </summary>
        public static IEnumerable<TResult> PagingQuery<TDb, TResult>(this IReadOnlyList<TDb> dbs, Func<TDb, IQueryable<TResult>> query, IPagingQueryPara para) 
            where TDb : ShardingDbContext
        {
            if (!para.QueryTotal)
            {
                return dbs.PagingQuery(query, (para.PageIndex - 1) * para.PageSize, para.PageSize);
            }

            var pageQry = DoPagingQuery(dbs, query, (para.PageIndex - 1) * para.PageSize, para.PageSize, para.QueryTotal, out var count);
            para.Count = count;
            return pageQry;
        }

        /// <summary>
        /// 按分页参数查询结果
        /// </summary>
        public static List<TResult> ToPagingList<TResult>(this IQueryable<TResult> source, IPagingQueryPara para)
        {
            if (!para.QueryTotal)
            {
                return source.Skip((para.PageIndex - 1) * para.PageSize).Take(para.PageSize).ToList();
            }

            para.Count = source.Count();
            var pageQry = source.Skip((para.PageIndex - 1) * para.PageSize).Take(para.PageSize).ToList();
            return pageQry;
        }

        /// <summary>
        /// DateTime.Today 在当前ScopeType下的ContextScope。
        /// </summary>
        public static ContextScope Today(this ShardScopeType scopeType)
        {
            return ContextScope.Create(scopeType, DateTime.Today);
        }

        #endregion
    }
}
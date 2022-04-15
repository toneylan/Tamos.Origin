using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.DataPersist;

public static class DbAccessExtend
{
    #region Fill property

    /// <summary>
    /// 依据列表数据的关联Id，查询关联的记录并填充到相应的属性值。
    /// </summary>
    public static void FillProperty<TSource, TDTO>(this IReadOnlyCollection<TSource>? list, Expression<Func<TSource, TDTO>> expressionProp,
        Func<TSource, long> funcPropId, IFastAccessor<TDTO> accessor)
        where TDTO : IGeneralEntity
    {
        list.FillProperty(expressionProp, funcPropId, accessor.Get);
    }

    /// <summary>
    /// 依据列表数据的关联Id，查询关联的记录，以执行setProp。
    /// </summary>
    [return: NotNullIfNotNull("list")]
    public static IReadOnlyCollection<TSource>? FillPropSet<TSource, TDTO>(this IReadOnlyCollection<TSource>? list, Func<TSource, long> funcRelId,
        IFastAccessor<TDTO> accessor, Action<TSource, TDTO> setProp) where TDTO : IGeneralEntity
    {
        return list.FillPropSet(funcRelId, relIds => accessor.Get(relIds.ToList()), setProp);
    }

    /*public static void FillSubItems<TEntity, TSub>(this IReadOnlyCollection<TEntity>? list, EntitySubRelation<TEntity, TSub> rel, IFastAccessor<TSub> accessor)
        where TSub : IGeneralEntity
    {
        if (list.IsNull()) return;


    }*/

    /*/// <summary>
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
    }*/

    #endregion

    #region 对比数据修改

    /// <summary>
    /// Apply data compare，then set add/update/delete operation.
    /// </summary>
    public static void ApplyDataCompare<T>(this IReadOnlyCollection<T> toItems, IReadOnlyCollection<T> dbItems, Func<T, DbSet<T>> getDbSet, ComparedSetter<T>? setter = null)
        where T : class, IComparablePO<T>
    {
        if (toItems.Count == 0)
        {
            LogService.WarnFormat("Warning: ApplyDataCompare toItems is empty，this will cause delete {0} dbItems.\r\n{1}", dbItems.Count, Environment.StackTrace);
        }

        //check to items
        foreach (var toItem in toItems)
        {
            var dbItem = dbItems.FirstOrDefault(x => x.SameAs(toItem));
            if (dbItem == null) //Add
            {
                if (setter?.OnAdd == null || setter.OnAdd(toItem)) getDbSet(toItem).Add(toItem);
            }
            else //update
            {
                dbItem.Update(toItem, setter);
            }
        }

        //check need delete items
        foreach (var item in dbItems.Where(d => !toItems.Any(t => t.SameAs(d))))
        {
            if (setter?.OnDelete == null || setter.OnDelete(item)) getDbSet(item).Remove(item);
        }
    }

    /// <summary>
    /// Save comparable PO data
    /// </summary>
    public static int ComparableSave<TDTO, TPO>(this FastAccessor<TDTO, TPO> accessor, IReadOnlyCollection<TPO> toItems, Expression<Func<TPO, bool>> queryDbItems)
        where TDTO : IGeneralEntity where TPO : class, IComparablePO<TPO>
    {
        var db = accessor.Db;
        var set = db.Set<TPO>();
        var dbItems = set.Where(queryDbItems).ToList();
        toItems.ApplyDataCompare(dbItems, _ => set);

        return db.SaveChanges();
    }

    /*/// <summary>
    /// 包含子项数据的修改对比，并执行增、改、删操作
    /// </summary>
    protected void CompareHierarchyData<T, TSub>(ICollection<T> dbData, ICollection<T> syncData, Func<T, DbSet<T>> getDbSet, Func<T, DbSet<TSub>> getSubDbSet
        , HierarchyCompSetter<T, TSub> setter = null)
        where T : class, IHierarchyComparable<T, TSub> 
        where TSub : class, IDataComparable<TSub>//, IGeneralSubEntity
    {
        if (syncData == null)
        {
            if (dbData?.Count > 0) LogService.ErrorFormat("比对数据修改时syncData为null，dbData count:{0}\r\n{1}", dbData.Count, Environment.StackTrace);
            return;
        }
        if (dbData == null) dbData = new List<T>();

        //检查修改
        foreach (var newItem in syncData)
        {
            var dbItem = dbData.FirstOrDefault(x => x.SameAs(newItem));
            var newSubs = newItem.SubItems ?? new List<TSub>(); //明细
            //---新增
            if (dbItem == null)
            {
                if (setter?.OnAdd != null && !setter.OnAdd(newItem)) continue;
                getDbSet(newItem).Add(newItem);
                if (newSubs.Count > 0) getSubDbSet(newItem).AddRange(newSubs);
            }
            else //---修改
            {
                dbItem.Update(newItem, setter);

                //子项更新
                var dbSubs = dbItem.SubItems ?? new List<TSub>();
                CompareDataChange(dbSubs, newSubs, s => getSubDbSet(dbItem), setter?.GetSubUpdateSet(newItem));
            }
        }

        //检查删除项
        foreach (var delItem in dbData.Where(d => !syncData.Any(s => s.SameAs(d))))
        {
            if (setter?.OnDelete != null && !setter.OnDelete(delItem)) continue;

            getDbSet(delItem).Remove(delItem);
            //同时删除子项
            var delSubs = delItem.SubItems;
            if (delSubs?.Count > 0) getSubDbSet(delItem).RemoveRange(delSubs);
        }
    }*/

    #endregion

    #region Build query

    //private static readonly MethodInfo ContainsMethod = typeof(List<long>).GetMethod("Contains");

    /*private static readonly MethodInfo MethodContains = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
        .Single(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2);*/

    /*/// <summary>
    /// build query of id in: x => ids.Contains(x.SomeId)
    /// </summary>
    private static IQueryable<T> QueryIdIn<T>(IQueryable<T> query, IReadOnlyCollection<long> ids, Expression<Func<T, long>> getIdExp)
    {
        var callExp = Expression.Call(Expression.Constant(ids), ContainsMethod, getIdExp.Body);
        
        return query.Where(Expression.Lambda<Func<T, bool>>(callExp, getIdExp.Parameters[0]));
    }*/

    #endregion
}
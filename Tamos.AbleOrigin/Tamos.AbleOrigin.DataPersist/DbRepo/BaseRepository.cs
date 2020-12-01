using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Tamos.AbleOrigin.Log;
using Tamos.AbleOrigin.ServiceBase;

namespace Tamos.AbleOrigin.DataPersist
{
    public abstract class BaseRepository : BaseServiceComponent
    {
        /// <summary>
        /// 创建DbCreater（主要用于子类调用，会确保Db的静态构造函数被调用-RegDb）
        /// </summary>
        protected ShardingDbCreater<T> DbCreater<T>() where T : ShardingDbContext
        {
            ShardingDbContext.EnsureDbTypeReg(typeof(T));

            return GetComponent<ShardingDbCreater<T>>();
        }

        #region 对比数据修改

        /// <summary>
        /// 对比同步数据修改，并执行增、改、删操作
        /// </summary>
        protected void CompareDataChange<T>(ICollection<T> dbData, ICollection<T> syncData, Func<T, DbSet<T>> getDbSet, ComparedSetter<T> setter = null)
            where T : class, IDataComparable<T>
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
                if (dbItem == null) //新增
                {
                    if (setter?.OnAdd == null || setter.OnAdd(newItem)) getDbSet(newItem).Add(newItem);
                }
                else //修改
                {
                    dbItem.Update(newItem, setter);
                }
            }

            //检查删除项
            foreach (var item in dbData.Where(d => !syncData.Any(s => s.SameAs(d))))
            {
                if (setter?.OnDelete == null || setter.OnDelete(item)) getDbSet(item).Remove(item);
            }
        }

        /// <summary>
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
        }

        #endregion

        #region Db mapping reflect

        [Obsolete("改用DbContext.GetDbMap")]
        protected internal static string TableName<T>()
        {
            var type = typeof (T);
            var attr = type.GetCustomAttribute<TableAttribute>();
            return attr?.Name;
        }

        [Obsolete("改用DbContext.GetDbMap")]
        protected internal static string ColumnName<T>(Expression<Func<T, object>> propFun)
        {
            var propExpression = propFun.Body is UnaryExpression exp ? exp.Operand as MemberExpression : propFun.Body as MemberExpression;
            var prop = propExpression?.Member as PropertyInfo;
            if (prop == null) return null;

            var attr = prop.GetCustomAttribute<ColumnAttribute>();
            return attr?.Name;
        }
        
        #endregion
    }
}
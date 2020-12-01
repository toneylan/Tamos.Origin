using System;
using System.Collections.Generic;
using System.Linq;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.DataPersist
{
    /// <summary>
    /// 实现Id为主键的数据，Db记录的快速存取。<br/>
    /// 轻量级创建，如一个BaseServiceComponent类中，可同时为多个表创建Accessor，DbContext被缓存与共享。
    /// </summary>
    public abstract class BaseFastAccessor : IDisposable
    {
        #region Data write

        protected bool DoUpdate<TEntity>(BaseDbContext db, long id, Action<TEntity> setDbItem) where TEntity : class, IGeneralEntity
        {
            var dbItem = db.Set<TEntity>().FirstOrDefault(x => x.Id == id);
            if (dbItem == null) return false;

            setDbItem(dbItem);
            db.SaveChanges();
            return true;
        }

        protected bool DoDelete<TEntity>(BaseDbContext db, long id, Action<TEntity> afterDel = null) where TEntity : class, IGeneralEntity
        {
            var set = db.Set<TEntity>();
            var delItem = set.FirstOrDefault(x => x.Id == id);
            if (delItem == null) return false;

            set.Remove(delItem);
            if (db.SaveChanges() <= 0) return false;

            afterDel?.Invoke(delItem);
            return true;
        }

        protected bool DoDelete<TEntity>(BaseDbContext db, List<long> ids) where TEntity : class, IGeneralEntity
        {
            var set = db.Set<TEntity>();
            var delItems = set.Where(x => ids.Contains(x.Id)).ToList();
            if (delItems.Count == 0) return false;

            set.RemoveRange(delItems);
            return db.SaveChanges() > 0;
        }

        #endregion

        public virtual void Dispose()
        {

        }
    }
}
using System;

namespace Tamos.AbleOrigin.DataPersist
{
    /// <summary>
    /// 数据比对的上下文
    /// </summary>
    public class ComparedContext
    {

    }

    /// <summary>
    /// 同步行为设置
    /// </summary>
    public class ComparedSetter<T> : ComparedContext
    {
        /// <summary>
        /// 数据库添加操作前执行，返回false将取消
        /// </summary>
        public Func<T, bool> OnAdd { get; set; }

        /// <summary>
        /// 数据库删除操作前执行，返回false将取消
        /// </summary>
        public Func<T, bool> OnDelete { get; set; }
    }

    public class HierarchyCompSetter<T, TSub> : ComparedSetter<T>
    {
        /// <summary>
        /// 当存在主记录(T为SyncItem)，追加明细
        /// </summary>
        public Func<T, TSub, bool> OnAppendSub { get; set; }

        /// <summary>
        /// 当存在主记录(T为SyncItem)，移除数据库中多余明细
        /// </summary>
        public Func<T, TSub, bool> OnRemoveSub { get; set; }

        #region SubSetter

        private T _mainItem; //保存主同步记录
        private ComparedSetter<TSub> _subSetter;

        /// <summary>
        /// 构造子项的更新设置
        /// </summary>
        internal ComparedSetter<TSub> GetSubUpdateSet(T syncItem)
        {
            _mainItem = syncItem;
            if (_subSetter == null)
            {
                _subSetter = new ComparedSetter<TSub>();
                if (OnAppendSub != null) _subSetter.OnAdd = sub => OnAppendSub(_mainItem, sub);
                if (OnRemoveSub != null) _subSetter.OnDelete = sub => OnRemoveSub(_mainItem, sub);
            }
            return _subSetter;
        }

        #endregion
    }

    /*public enum DbAction
    {
        Add = 1,
        Delete = 2
    }*/
}
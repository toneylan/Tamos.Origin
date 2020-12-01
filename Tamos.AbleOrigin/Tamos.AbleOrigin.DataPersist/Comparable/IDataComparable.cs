using System.Collections.Generic;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.DataPersist
{
    /// <summary>
    /// 指导数据的比对操作
    /// </summary>
    public interface IDataComparable<in T> : IGeneralEntity
    {
        bool SameAs(T item);

        void Update(T syncItem, ComparedContext context);
    }

    /// <summary>
    /// 层级数据的比对操作
    /// </summary>
    public interface IHierarchyComparable<in T, TSub> : IDataComparable<T>, IGeneralParentEntity<TSub>
        //where TSub : IGeneralSubEntity
    {
    }
}
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.DataPersist
{
    /// <summary>
    /// PO is comparable, to determine db operate
    /// </summary>
    public interface IComparablePO<in T> : IGeneralEntity
    {
        bool SameAs(T item);

        void Update(T toItem, ComparedContext? context);
    }

    /*/// <summary>
    /// 层级数据的比对操作
    /// </summary>
    public interface IHierarchyComparable<in T, TSub> : IDataComparable<T>, IGeneralParentEntity<TSub>
    {
    }*/
}
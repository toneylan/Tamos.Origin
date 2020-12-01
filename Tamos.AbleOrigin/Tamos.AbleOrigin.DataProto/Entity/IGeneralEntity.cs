using System.Collections.Generic;

namespace Tamos.AbleOrigin.DataProto
{
    /// <summary>
    /// 定义实体或者DTO的普遍属性（如Id），便于进行常规化操作。
    /// </summary>
    public interface IGeneralEntity
    {
        /// <summary>
        /// 对象自身Id
        /// </summary>
        long Id { get; set; }
    }

    /// <summary>
    /// 子级对象，隶属某个父级对象。
    /// </summary>
    public interface IGeneralSubEntity : IGeneralEntity
    {
        /// <summary>
        /// 父级对象Id
        /// </summary>
        long ParentId { get; set; }
    }

    /// <summary>
    /// 带子级数据的对象。
    /// </summary>
    public interface IGeneralParentEntity<TSub> : IGeneralEntity //where TSub : IGeneralSubEntity
    {
        /// <summary>
        /// 子级数据列表
        /// </summary>
        List<TSub> SubItems { get; set; }
    }
}
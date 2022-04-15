using System.Collections.Generic;
using System.Linq;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.DataPersist
{
    /*public class BondObj<T, TSub> where T : class, IGeneralParentEntity<TSub> //where TSub : IGeneralSubEntity
    {
        public T Data { get; set; }
        public IEnumerable<TSub> SubItems { get; set; }

        *//*public BondObj(T data, IEnumerable<TSub> subs)
        {
            Data = data;
            SubItems = subs;
        }*//*

        /// <summary>
        /// 设置子列表并返回Data
        /// </summary>
        public T CvtObj()
        {
            Data.SubItems = SubItems?.ToList();
            return Data;
        }
    }*/

    /// <summary>
    /// EF Core目前不支持GroupJoin，此对象保存Join查询结果，然后再执行Group。
    /// </summary>
    public class JoinObj<TOuter, TInner> //where TOuter : class
    {
        public TOuter Data { get; set; }
        
        /// <summary>
        /// Left join 时可能为null
        /// </summary>
        public TInner Item { get; set; }
    }

    /*public class GeneralEntComparer : IEqualityComparer<IGeneralEntity>
    {
        public bool Equals(IGeneralEntity x, IGeneralEntity y)
        {
            throw new System.NotImplementedException();
        }

        public int GetHashCode(IGeneralEntity obj)
        {
            throw new System.NotImplementedException();
        }
    }*/
}
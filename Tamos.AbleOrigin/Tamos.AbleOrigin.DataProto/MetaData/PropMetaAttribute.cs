using System;

namespace Tamos.AbleOrigin.DataProto
{
    /// <summary>
    /// 定义属性的元信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropMetaAttribute : Attribute
    {
        /// <summary>
        /// 中文名称
        /// </summary>
        public string CnName { get; set; }

        /// <summary>
        /// 是否必需的属性。数字大于0，字符串非空……
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// 最大值或长度（字符串时）
        /// </summary>
        public int MaxValue { get; set; }

        //****** 增加条件后，检查PropertyMeta.IsValid 方法 ******

        public PropMetaAttribute(string cnName, bool isRequired = false)
        {
            CnName = cnName;
            IsRequired = isRequired;
        }
    }
}
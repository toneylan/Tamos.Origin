using System;
using Tamos.AbleOrigin.Common;

namespace Tamos.AbleOrigin.DataProto
{
    /// <summary>
    /// 提供属性检查功能
    /// </summary>
    internal class PropertyValidator<TProp>
    {
        /// <summary>
        /// 默认属性值验证
        /// </summary>
        public virtual string Validate(PropMetaAttribute attrSet, TProp propVal)
        {
            if (attrSet.IsRequired && propVal == null) return $"未设置{attrSet.CnName}";
            
            return null;
        }
    }

    #region Specific validator

    internal class IntPropValidator : PropertyValidator<int>
    {
        public override string Validate(PropMetaAttribute attrSet, int propVal)
        {
            if (attrSet.IsRequired && propVal <= 0) return $"{attrSet.CnName}需大于0";
            if (attrSet.MaxValue != 0 && propVal > attrSet.MaxValue) return $"{attrSet.CnName}不能大于{attrSet.MaxValue}";

            return null;
        }
    }

    internal class LongPropValidator : PropertyValidator<long>
    {
        public override string Validate(PropMetaAttribute attrSet, long propVal)
        {
            if (attrSet.IsRequired && propVal <= 0) return $"{attrSet.CnName}需大于0";
            if (attrSet.MaxValue != 0 && propVal > attrSet.MaxValue) return $"{attrSet.CnName}不能大于{attrSet.MaxValue}";

            return null;
        }
    }

    internal class StrPropValidator : PropertyValidator<string>
    {
        public override string Validate(PropMetaAttribute attrSet, string propVal)
        {
            if (attrSet.IsRequired && propVal.IsNull()) return $"{attrSet.CnName}不能为空";
            if (attrSet.MaxValue != 0 && propVal?.Length > attrSet.MaxValue) return $"{attrSet.CnName}长度不能大于{attrSet.MaxValue}";

            return null;
        }
    }

    #endregion
}
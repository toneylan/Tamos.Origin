using System;

namespace Tamos.AbleOrigin.DataProto
{
    /// <summary>
    /// DB 中json字段对应的class基类
    /// </summary>
    public class BaseJsonField<T> : IEquatable<T> where T : BaseJsonField<T>
    {
        private bool _isChanged;

        /// <summary>
        /// 当修改了映射的Json对象中属性时，标记对象为“Changed”，以触发数据库更新。<br/>
        /// 否则目前的默认设置下，Json对象的属性变化，无法触发EF Core change tracking，具体参见配置：MySqlCommonJsonChangeTrackingOptions
        /// </summary>
        public void SetChanged()
        {
            _isChanged = true;
        }

        /// <summary>
        /// Internal use，用于控制EF Core change tracking。false 才能触发数据库更新。
        /// </summary>
        public bool Equals(T? other)
        {
            if (other == null) return false;

            return !_isChanged && !other._isChanged && Equals(this, other);
        }
    }
}
using System;
using System.Runtime.Serialization;

namespace Tamos.AbleOrigin.DataProto
{
    /// <summary>
    /// 枚举值包装器
    /// </summary>
    [DataContract]
    public struct GeneralEnum
    {
        /// <summary>
        /// 枚举short值
        /// </summary>
        [DataMember(Order = 1)]
        public short Value { get; set; }

        /*public T To<T>() where T : Enum
        {
            return Value as T;
        }*/

        #region operator

        public static implicit operator GeneralEnum(Enum val)
        {
            return new() {Value = val.GetShort()};
        }

        public static implicit operator GeneralEnum(short val)
        {
            return new() {Value = val};
        }

        #endregion
    }
}
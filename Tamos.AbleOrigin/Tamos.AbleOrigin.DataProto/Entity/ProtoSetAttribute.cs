using System;

namespace Tamos.AbleOrigin.DataProto
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ProtoSetAttribute : Attribute
    {
        /// <summary>
        /// 基类中包含时的Tag
        /// </summary>
        public int Tag { get; }

        /// <summary>
        /// 标记基类，使其序列化时能包含父类属性。适用于无法在基类使用ProtoInclude的场景。
        /// </summary>
        public Type BaseType { get; }

        public ProtoSetAttribute(int tag, Type baseType)
        {
            Tag = tag;
            BaseType = baseType;
        }
    }
}
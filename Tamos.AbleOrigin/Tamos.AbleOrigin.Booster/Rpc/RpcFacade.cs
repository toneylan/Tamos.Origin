using System.Reflection;
using ProtoBuf.Meta;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.Booster
{
    public static class RpcFacade
    {
        #region Protobuf-net

        /// <summary>
        /// 在DTO库中，配置通过ProtoSet设置的继承关系
        /// </summary>
        public static void ApplyProtoSet(this Assembly dtoAssembly)
        {
            var typeModel = RuntimeTypeModel.Default;
            foreach (var type in dtoAssembly.GetExportedTypes())
            {
                var attr = type.GetCustomAttribute<ProtoSetAttribute>();
                if (attr?.BaseType == null) continue;

                typeModel[attr.BaseType].AddSubType(attr.Tag, type);
            }
        }

        #endregion
    }
}
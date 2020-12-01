using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tamos.AbleOrigin.Common;

namespace Tamos.MetaCoding.GeneratorDesign
{
	/// <summary>
	/// 类型（Class）的成员基类
	/// </summary>
    public abstract class BaseTypeMember
    {
        private static readonly Dictionary<Type, string> TypeNameCache = new Dictionary<Type, string>();


        /*internal static string GetParameterName(ParameterInfo para)
        {
            string des = null;
            if (para.IsOut) des = "out ";
            else if (para.ParameterType.IsByRef) des = "ref ";
            return $"{des}{para.Name}";
        }*/

        #region Type name desc

        /// <summary>
        /// 连结参数类型
        /// </summary>
        internal string JoinParaType(IEnumerable<ParameterInfo> paras)
        {
            return string.Join(", ", paras.Select(GetParaType));
        }
        
        internal string GetParaType(ParameterInfo para)
        {
            return GetTypeName(para.IsOut ? para.ParameterType.GetElementType() : para.ParameterType);
        }

        /// <summary>
        /// 解析出类型定义string
        /// </summary>
        public string GetTypeName(Type type)
        {
            if (TypeNameCache.TryGetValue(type, out var tpName)) return tpName;

            //-- GenericType
            if (type.IsGenericType)
            {
                var nullableType = Nullable.GetUnderlyingType(type); //if NullableType
                if (nullableType != null)
                {
                    tpName = GetTypeName(nullableType) + "?";
                }
                else
                {
                    tpName = string.Format("{0}{1}<{2}>", type.Namespace?.StartsWith("System") == true ? null : type.Namespace + ".",
                        type.Name.Substring(0, type.Name.IndexOf('`')), string.Join(", ", type.GetGenericArguments().Select(GetTypeName)));    
                }
                
                return TypeNameCache.SetValue(type, tpName);
            }

            switch (type.Name) //Friendly name
            {
                case "Boolean":
                    return "bool";
                case "String":
                    return "string";
                case "Int32":
                    return "int";
                case "Int64":
                    return "long";
                case "Decimal":
                    return "decimal";
                case "Object":
                    return "object";
                case "Void":
                    return "void";
            }

            return type.FullName ?? type.Name;
        }

        #endregion
    }
}
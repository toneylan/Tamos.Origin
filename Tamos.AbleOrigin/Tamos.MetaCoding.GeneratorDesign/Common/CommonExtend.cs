using System;
using System.Collections.Generic;
using System.Linq;
using Tamos.AbleOrigin;

namespace Tamos.MetaCoding.GeneratorDesign
{
    internal static class CommonExtend
    {
        public static string NoNull(this string src)
        {
            return src ?? string.Empty;
        }

        #region Type des

        private static readonly Dictionary<Type, string> TypeNameCache = new();

        /// <summary>
        /// 是否类或接口
        /// </summary>
        internal static bool IsClass(this Type type)
        {
            return type.IsClass && !type.IsString() || type.IsInterface;
        }

        public static bool IsString(this Type type)
        {
            return type.Name == "String";
        }

        public static bool IsGenRes(this Type type)
        {
            return type.IsClass && type.GetInterface("IGeneralResObj") != null;
        }

        /// <summary>
        /// 解析出类型定义string
        /// </summary>
        public static string GetName(this Type type)
        {
            if (TypeNameCache.TryGetValue(type, out var tpName)) return tpName;

            //-- GenericType
            if (type.IsGenericType)
            {
                var nullableType = Nullable.GetUnderlyingType(type); //if NullableType
                if (nullableType != null)
                {
                    tpName = nullableType.GetName() + "?";
                }
                else
                {
                    tpName = string.Format("{0}{1}<{2}>", type.Namespace?.StartsWith("System") == true ? null : type.Namespace + ".",
                        type.Name.Substring(0, type.Name.IndexOf('`')), string.Join(", ", type.GetGenericArguments().Select(GetName)));    
                }
                
                return TypeNameCache.SetValue(type, tpName);
            }

            //Friendly name
            return type.Name switch
            {
                "Boolean" => "bool",
                "String" => "string",
                "Int32" => "int",
                "Int64" => "long",
                "Decimal" => "decimal",
                "Object" => "object",
                "Void" => "void",
                _ => type.GetFullName()
            };
        }

        #endregion

        #region T4Template

        public static void PushIndent(this IT4Template writer)
        {
            writer.PushIndent("    ");
        }

        public static void BeginSubWrite(this IT4Template writer)
        {
            writer.WriteLine("{");
            writer.PushIndent("    ");
        }

        public static void EndSubWrite(this IT4Template writer)
        {
            writer.PopIndent();
            writer.WriteLine("}");
        }

        #endregion
    }
}
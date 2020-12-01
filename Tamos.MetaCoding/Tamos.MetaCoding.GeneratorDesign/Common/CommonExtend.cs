using System;
using System.Linq;

namespace Tamos.MetaCoding.GeneratorDesign
{
    internal static class CommonExtend
    {
        public static string NoNull(this string src)
        {
            return src ?? string.Empty;
        }

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
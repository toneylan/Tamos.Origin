using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tamos.MetaCoding.GeneratorDesign
{
	/// <summary>
	/// 类型（Class）的成员基类
	/// </summary>
    public abstract class BaseTypeMember
    {
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
            return (para.IsOut ? para.ParameterType.GetElementType() : para.ParameterType)?.GetName() ?? string.Empty;
        }

        /// <summary>
        /// 改用扩展方法
        /// </summary>
        public string GetTypeName(Type type) => type.GetName();

        #endregion
    }
}
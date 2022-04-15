using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Tamos.AbleOrigin;

namespace Tamos.MetaCoding.GeneratorDesign
{
    public class MemberMethod : BaseTypeMember
    {
        #region Props

        public const string ErrorParaName = "error";

        public MethodInfo MethodInfo { get; set; }
        public string RawInterface { get; set; }
        
        /// <summary>
        /// Proxy的返回类型，可能与接口方法的ReturnType不一样，因为存在参数封装。
        /// </summary>
        public string ProxyResTypeName { get; set; }
        
        private string? _customName;
        public string Name
        {
            get => _customName ?? MethodInfo.Name;
            set => _customName = value;
        }
        
        /// <summary>
        /// 是否异步方法
        /// </summary>
        public bool IsAsyncMethod { get; }

        #endregion
        
        public MemberMethod(MethodInfo method)
        {
            MethodInfo = method;
            IsAsyncMethod = ReturnType.IsGenericType && ReturnType.GetGenericTypeDefinition() == typeof(ValueTask<>);
        }

        #region Return type & tmpl

        public Type ReturnType => MethodInfo.ReturnType;
        protected Type RealReturnType => IsAsyncMethod ? ReturnType.GenericTypeArguments[0] : ReturnType;
        //public bool IsReturnGenRes => ReturnType.IsGenRes();

        /// <summary>
        /// Async方式时为 ValueTask->ProxyResTypeName
        /// </summary>
        internal string ProxyResTypeNameFull => IsAsyncMethod ? $"ValueTask<{ProxyResTypeName}>" : ProxyResTypeName;
        
        public bool IsVoidReturn => MethodInfo.ReturnType.Name == "Void";
        
        /// <summary>
        /// 是否能返回错误信息（返回string则代表错误信息，对外接口不适合这样设计）
        /// </summary>
        public virtual bool AbleReturnError()
        {
            //byOutPara = Parameters.FirstOrDefault(IsOutError)?.Name;
            return ReturnType.IsString();
        }

        #endregion

        #region Parameter type

        private ParameterInfo[]? _parameters;
        public ParameterInfo[] Parameters => _parameters ??= MethodInfo.GetParameters();

        /// <summary>
        /// 保持顺序的 in/out 参数
        /// </summary>
        internal IEnumerable<ParameterInfo> FormOfParas(bool isOut) => Parameters.Where(p => p.IsOut == isOut);

        public bool IsOutError(ParameterInfo para)
        {
            return para.IsOut && para.Name == ErrorParaName && para.ParameterType.GetElementType()?.IsString() == true;
        }
        
        #endregion

        #region Method signature reflect

        /// <summary>
        /// 原始定义接口名称
        /// </summary>
        public string GetRawInterface()
        {
            var decTpName = MethodInfo.DeclaringType?.Name;
            if (decTpName != null && decTpName != RawInterface) return MethodInfo.DeclaringType.FullName;
            return RawInterface;
        }

        public string GetParameterDes(ParameterInfo para)
        {
            return string.Format("{0}{1} {2}", para.IsOut ? "out " : (para.ParameterType.IsByRef ? "ref " : null),
                GetParaType(para), para.Name);
        }

        #endregion
        
        /*public override void RenderCode(CodeWriter writer)
        {
            //write method content
            if (MethodInfo.IsPrivate)
                writer.Write("private ");
            else if (MethodInfo.IsPublic)
                writer.Write("public ");
            if (MethodInfo.IsAbstract)
                writer.Write("abstract ");
            if (MethodInfo.IsStatic)
                writer.Write("static ");
            if (MethodInfo.IsVirtual)
                writer.Write("virtual ");
        }*/
    }
}
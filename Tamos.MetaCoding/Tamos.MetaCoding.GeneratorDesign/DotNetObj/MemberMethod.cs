using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Tamos.AbleOrigin.Common;

namespace Tamos.MetaCoding.GeneratorDesign
{
    public class MemberMethod : BaseTypeMember
    {
        #region Props

        public const string ErrorParaName = "error";

        public MethodInfo MethodInfo { get; set; }
        public string RawInterface { get; set; }
        
        public string ProxyResTypeName { get; set; }
        
        private string _customName;
        public string Name
        {
            get => _customName ?? MethodInfo.Name;
            set => _customName = value;
        }

        #endregion

        #region Parameter & Return
        
        public Type ReturnType => MethodInfo.ReturnType;

        public bool IsVoidReturn => MethodInfo.ReturnType.Name == "Void";

        private ParameterInfo[] _parameters;
        public ParameterInfo[] Parameters => _parameters ?? (_parameters = MethodInfo.GetParameters());

        /// <summary>
        /// 保持顺序的 in/out 参数
        /// </summary>
        internal IEnumerable<ParameterInfo> FormOfParas(bool isOut) => Parameters.Where(p => p.IsOut == isOut);

        /// <summary>
        /// 方法是否能返回错误信息（返回值为string且无out error，则认为代表错误信息）
        /// </summary>
        public virtual bool AbleReturnError(out string byOutPara)
        {
            byOutPara = Parameters.FirstOrDefault(x => x.IsOut && x.Name == ErrorParaName && x.ParameterType.GetElementType().IsString())?.Name;
            return byOutPara.NotNull() || ReturnType.IsString();
        }

        #endregion

        public MemberMethod(MethodInfo method)
        {
            MethodInfo = method;
        }
        
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
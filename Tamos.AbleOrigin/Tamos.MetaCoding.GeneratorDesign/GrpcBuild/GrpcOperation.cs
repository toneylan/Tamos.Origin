using System;
using System.Linq;
using System.Reflection;
using Tamos.AbleOrigin;

namespace Tamos.MetaCoding.GeneratorDesign
{
    public class GrpcOperation : MemberMethod
    {
        internal const string Var_Para = "_para_";
        internal const string Var_MsgWrp = "RpcMsgWrp";
        internal const int MsgGenCount = 6; //泛型消息的类参最大个数

        internal string ProxyReqTypeName { get; set; }
        internal ParaCaseType ParaCase { get; set; }
        internal ParaCaseType ResCase { get; set; }

        private bool WrappedPara => ParaCase == ParaCaseType.Wrap;
        internal bool ResRawType => ResCase == ParaCaseType.OneRaw;

        public GrpcOperation(MethodInfo method) : base(method)
        {
        }

        #region Res type

        /// <summary>
        /// Build error res
        /// </summary>
        public string ErrorRes()
        {
            return ResRawType ? $"new {ProxyResTypeName} {{ ErrorMsg = e.Message }}" : "e.Message";
        }

        #endregion

        //调用原始接口的参数
        internal string GetRawCallParas()
        {
            if (Parameters.Length == 0) return string.Empty;
            var tag = 0;
            return string.Join(", ", Parameters.Select(p => p.IsOut ? $"out var {p.Name}"
                : WrappedPara ? $"{Var_Para}.P{++tag}" : Var_Para));
        }

        internal string GetProxyCallParas()
        {
            if (ParaCase == ParaCaseType.OneRaw) return FormOfParas(false).First().Name!;
            if (ParaCase == ParaCaseType.No) return $"new {ProxyReqTypeName}()";
            
            var tag = 0;
            return string.Format("new {0} {{{1}}}", ProxyReqTypeName, string.Join(", ", FormOfParas(false).Select(p => $"P{++tag} = {p.Name}")));
        }

        #region Out 参数

        //out 参数赋值
        internal string? WriteOutParaAssign(IT4Template writer)
        {
            if (ResCase != ParaCaseType.WrapOut) return null;

            var tag = 1;
            writer.Write("                ");
            foreach (var p in FormOfParas(true))
            {
                //if (IsOutError(p)) continue;
                writer.Write($"{p.Name} = _res_.P{++tag}; "); //From Response
            }
            writer.WriteLine(string.Empty);

            //onException
            return string.Join(" ", FormOfParas(true).Select(p => $"{p.Name} = default;"));
        }

        public override bool AbleReturnError()
        {
            return ResCase == ParaCaseType.OneRaw || RealReturnType.IsGenRes() || base.AbleReturnError();
        }

        #endregion

        #region DefineMsgType

        /// <summary>
        /// 方法初始化
        /// </summary>
        internal void DefineMsgType(IT4Template writer)
        {
            #region Req参数

            var reqParas = Parameters.Separate(x => !x.IsOut, out var outParas);
            if (reqParas.Count == 0)
            {
                ProxyReqTypeName = $"{Var_MsgWrp}";
                ParaCase = ParaCaseType.No;
            }
            else if (reqParas.Count == 1)
            {
                var pt = reqParas[0].ParameterType;
                if (pt.IsClass())
                {
                    ParaCase = ParaCaseType.OneRaw;
                    ProxyReqTypeName = GetTypeName(pt);
                }
            }

            //包裹参数
            if (WrappedPara)
            {
                if (reqParas.Count <= MsgGenCount) ProxyReqTypeName = $"{Var_MsgWrp}<{JoinParaType(reqParas)}>";
                else //参数过多，构造类型
                {
                    ProxyReqTypeName = Name + "_Para";
                    //Type define
                    writer.WriteLine("[DataContract]");
                    writer.WriteLine($"internal class {ProxyReqTypeName}");
                    writer.BeginSubWrite();
                    var tag = 0;
                    foreach (var para in reqParas)
                    {
                        writer.WriteLine($"[DataMember(Order = {++tag})]");
                        writer.WriteLine($"public {GetParaType(para)} P{tag} {{ get; set; }}");
                    }
                    writer.EndSubWrite();
                    writer.WriteLine(null);
                }
            }

            #endregion

            //---Res 对象
            if (IsVoidReturn) ProxyResTypeName = Var_MsgWrp;
            else if (outParas.IsNull())
            {
                var realRetType = RealReturnType;
                if (realRetType.IsGenRes()) ResCase = ParaCaseType.OneRaw;
                ProxyResTypeName = ResRawType ? realRetType.GetName() : $"{Var_MsgWrp}<{realRetType.GetName()}>";
            }
            else //带out的多返回
            {
                if (outParas.Count >= MsgGenCount) throw new Exception($"{RawInterface}.{MethodInfo.Name}Out参数过多，无法Wrap");
                ProxyResTypeName = $"{Var_MsgWrp}<{GetTypeName(ReturnType)}, {JoinParaType(outParas)}>";
                ResCase = ParaCaseType.WrapOut;
            }

        }

        #endregion
    }

    public enum ParaCaseType
    {
        Wrap = 0,
        No,
        OneRaw,

        /// <summary>
        /// 带out参数的wrap
        /// </summary>
        WrapOut
    }
}
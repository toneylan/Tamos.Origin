﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tamos.AbleOrigin.Common;

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
            if (ParaCase == ParaCaseType.OneRaw) return FormOfParas(false).First().Name;
            if (ParaCase == ParaCaseType.No) return $"new {ProxyReqTypeName}()";
            
            var tag = 0;
            return string.Format("new {0} {{{1}}}", ProxyReqTypeName, string.Join(", ", FormOfParas(false).Select(p => $"P{++tag} = {p.Name}")));
        }

        #region Out 参数

        //out 参数赋值
        internal string WriteOutParaAssign(IT4Template writer, string outErrPara)
        {
            if (ResCase != ParaCaseType.WrapOut) return null;

            var tag = 1;
            writer.Write("                ");
            foreach (var p in FormOfParas(true))
            {
                writer.Write($"{p.Name} = _res_.P{++tag}; "); //From Response
            }
            writer.WriteLine(string.Empty);

            //onException
            return string.Join(" ", FormOfParas(true).Select(p => $"{p.Name} = {(p.Name == outErrPara ? "e.Message" : "default")};"));
        }

        public override bool AbleReturnError(out string byOutPara)
        {
            if (ResCase != ParaCaseType.OneRaw) return base.AbleReturnError(out byOutPara);
            
            byOutPara = null;
            return true;
        }

        #endregion

        #region DefineMsgType

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
            else if (outParas.IsNullOrEmpty())
            {
                if (ReturnType.IsClass && ReturnType.GetInterface("IGeneralResObj") != null) ResCase = ParaCaseType.OneRaw;
                ProxyResTypeName = ResCase == ParaCaseType.OneRaw ? GetTypeName(ReturnType) : $"{Var_MsgWrp}<{GetTypeName(ReturnType)}>";
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

    internal enum ParaCaseType
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
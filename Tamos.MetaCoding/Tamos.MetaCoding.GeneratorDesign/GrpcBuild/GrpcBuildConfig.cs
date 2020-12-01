using System.Collections.Generic;
using Tamos.AbleOrigin.Common;

namespace Tamos.MetaCoding.GeneratorDesign
{
    public class GrpcBuildConfig : BaseBuildConfig
    {
        public const string InfraNamespace = Utility.ProdBrand + ".AbleOrigin";

        public List<string> AssemblyInterface { get; set; }
        public string OutNamespace { get; set; }
        public string OutServiceName { get; set; }
        public bool OutSyncMethod { get; set; }

        /*public GrpcBuildConfig()
        {
        }*/

        public string ContractName => $"I{OutServiceName}Contract";
        public string ServiceProxyName => $"{OutServiceName}Proxy";
        public string ServiceImplName => $"{OutServiceName}Impl";
    }
}
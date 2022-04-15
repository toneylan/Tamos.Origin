using System.Collections.Generic;
using Tamos.AbleOrigin;

namespace Tamos.MetaCoding.GeneratorDesign
{
    public class CodeBuildConfig : BaseBuildConfig
    {
        public const string InfraNamespace = Utility.ProdBrand + ".AbleOrigin";

        public List<string> AssemblyInterface { get; set; }
        public string? OutNamespace { get; set; }
        public string OutServiceName { get; set; }
        
        public bool UseCallContext { get; set; }

        /*public CodeBuildConfig()
        {
        }*/

        public string ContractName => $"I{OutServiceName}Contract";
        public string ServiceProxyName => $"{OutServiceName}Proxy";
        public string ServiceImplName => $"{OutServiceName}Impl";
    }
}
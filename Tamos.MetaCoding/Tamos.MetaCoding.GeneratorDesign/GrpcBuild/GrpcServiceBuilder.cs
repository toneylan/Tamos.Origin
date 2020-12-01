using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Tamos.AbleOrigin.Common;
using Tamos.AbleOrigin.Reflect;
using Tamos.MetaCoding.GeneratorDesign.GrpcBuild;

namespace Tamos.MetaCoding.GeneratorDesign
{
    public class GrpcServiceBuilder
    {
        public GrpcBuildConfig Config { get; }
        public string ContractName => Config.ContractName;
        public string ServiceProxyName => Config.ServiceProxyName;

        public GrpcServiceBuilder(GrpcBuildConfig conf)
        {
            Config = conf;
        }

        private IEnumerable<Type> GetServiceInterfaceList()
        {
            return Config.AssemblyInterface.SelectMany(i => (string.IsNullOrEmpty(Config.AssemblyPath)
                ? Assembly.Load(i)
                : Assembly.LoadFrom(Path.Combine(Config.AssemblyPath, i + ".dll")))
                .GetTypes().Where(x => x.IsInterface));
        }

        internal static bool HasAttribute<T>(Type type) where T : Attribute
        {
            try
            {
                return type.GetCustomAttribute<T>(false) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Warning:" + e.Message);
                return false;
            }
        }

        #region BuildServiceCode

        protected string GetInterfaceUsing()
        {
            return string.Join(Environment.NewLine, Config.AssemblyInterface.Select(x => $"using {x};"));
        }

		public async Task BuildServiceCode()
	    {
            //创建ServiceContract
            var contractFile = new CodeFile(Path.Combine(Config.OutputPath, ContractName + ".Designer.cs"));
		    var contractType = contractFile.AddType(new DotNetType());

            //创建ServiceContract Implement
            var implFile = new CodeFile(Path.Combine(Config.OutputPath, Config.ServiceImplName + ".Designer.cs"));
            var implType = implFile.AddType(new DotNetType());

		    //创建Host service proxy
		    var proxyFile = new CodeFile(Path.Combine(Config.OutputPath, ServiceProxyName + ".Designer.cs"));
            var proxyType = proxyFile.AddType(new DotNetType());
            proxyType.InheritAndImpls = new List<string>();

            //implFile.UsingSection = proxyFile.UsingSection = GetInterfaceUsing();

            //反射各个接口方法
            var dicMethodCount = new Dictionary<string, int>();
            List<string> iocBaseImpl = null; //需要在IOC中注册的接口基类
            foreach (var defIterface in GetServiceInterfaceList())
		    {
                //---check if ignored
                if (defIterface.Name == ContractName) continue;
		        if (HasAttribute<CodeGenIgnoreAttribute>(defIterface)) continue;
		        var intfName = Config.AssemblyInterface.Contains(defIterface.Namespace) ? defIterface.Name : defIterface.FullName;

                //---interface methods
		        ICollection<MethodInfo> iMethods = defIterface.GetMethods();
		        var parentInf = defIterface.GetInterfaces();
		        if (parentInf.Length > 0) //有父接口
		        {
		            iMethods = iMethods.Concat(parentInf.SelectMany(x => x.GetMethods())).ToList(); //加上父级接口的方法
                    //是否IOC注册父接口
		            var regBaseAttr = defIterface.GetCustomAttribute<CodeGenRegBaseImplAttribute>(false);
                    if (regBaseAttr != null) iocBaseImpl = iocBaseImpl.NullableAdd(parentInf[0].FullName);
                }
                
                proxyType.InheritAndImpls.Add(intfName);
                proxyType.AddIocServices = iocBaseImpl;

		        foreach (var methodInfo in iMethods)
                {
                    var memMethod = new GrpcOperation(methodInfo) {RawInterface = intfName};
			        if (methodInfo.IsGenericMethod) continue; //不支持泛型方法

                    //统计方法数量，不支持重载，故构造不同名称
                    if (dicMethodCount.TryGetValue(methodInfo.Name, out var meCount))
			        {
			            dicMethodCount[methodInfo.Name] = ++meCount;
			            memMethod.Name += meCount;
			        }
			        else
			        {
			            dicMethodCount.Add(methodInfo.Name, 1);
			        }
			        
                    //add method
                    contractType.AddMember(memMethod);
                    implType.AddMember(memMethod);
                    proxyType.AddMember(memMethod);
                }
            }

            //输出文件
            await contractFile.WriteFile(new TmplServiceContract(), Config);
            await implFile.WriteFile(new TmplServiceImplement(), Config);
            await proxyFile.WriteFile(new TmplServiceProxy(), Config);
	    }
        
        #endregion
	}
}
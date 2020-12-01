using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using Tamos.AbleOrigin.Booster;
using Tamos.AbleOrigin.DataPersist;
using Tamos.MetaCoding.GeneratorDesign;
using Tamos.MetaCoding.GeneratorDesign.GrpcBuild;

namespace Tamos.AbleOrigin.UnitTest
{
    public class CodeGenTest
    {
        [Test]
        public void Perform()
        {
            
            var contractFile = new CodeFile("test.cs");
            var contractType = new DotNetType();
            contractFile.TypeList.Add(contractType);
            contractType.InheritAndImpls = new List<string>{"ICodeGenInterface"};

            var type = typeof(TestDb);
            contractType.AddMember(new GrpcOperation(type.GetMethod("Set")));
            contractType.AddMember(new GrpcOperation(type.GetMethod("Get")));
            contractType.AddMember(new GrpcOperation(type.GetMethod("DoWork")));
            contractType.AddMember(new GrpcOperation(type.GetMethod("Update")));

            var paras = new Dictionary<string, object>
            {
                ["config"] = new GrpcBuildConfig {OutNamespace = "OutNamespace", OutServiceName = "RpcSrv"},
                ["file"] = contractFile
            };

            var tmpl = new TmplServiceContract {Session = paras};
            var t2 = new TmplServiceImplement{Session = paras};
            var t3 = new TmplServiceProxy() {Session = paras};
            tmpl.Initialize();
            t2.Initialize();
            t3.Initialize();

            tmpl.TransformText();
            t2.TransformText();
            Console.WriteLine(t3.TransformText());
        }

        [Test]
        public void HashSpeed()
        {
            var intf = typeof(RpcMsgWrp).GetInterface("IGeneralResObj");
        }
    }

    public class TestDb : ShardingDbContext
    {
        static TestDb()
        {
            Console.WriteLine("TestDb static ctor");
        }

        protected TestDb(ContextScope scope, string connectionString) : base(scope, connectionString)
        {
        }

        protected TestDb(ContextScope scope, DbTransactionContext dbTran) : base(scope, dbTran)
        {
        }
    }
}
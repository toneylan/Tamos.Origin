namespace Tamos.MetaCoding.GeneratorDesign.GrpcBuild
{
    public partial class TmplServiceContract : IT4Template
    {

    }

    public partial class TmplServiceImplement : IT4Template
    {

    }

    public partial class TmplServiceProxy : IT4Template
    {

    }
}

#region ------- Net core 不支持 CallContext，避免编译错误而添加 ----------

namespace System.Runtime.Remoting.Messaging
{
    internal class CallContext
    {
        public static string LogicalGetData(string key)
        {
            return null;
        }
    }
}

#endregion
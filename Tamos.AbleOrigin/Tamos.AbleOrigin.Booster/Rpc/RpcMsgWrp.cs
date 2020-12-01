using System.Runtime.Serialization;
using ProtoBuf;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.Booster
{
    [DataContract]
    public class RpcMsgWrp : IGeneralResObj
    {
        [DataMember(Order = 20)]
        public string ErrorMsg { get; set; }
    }

    #region Msg1
    
    [ProtoContract]
    public class RpcMsgWrp<T1> : IGeneralResObj
    {
        [ProtoMember(1)]
        public T1 P1 { get; set; }
        
        [ProtoMember(20)]
        public string ErrorMsg { get; set; }
    }
    
    #endregion
    
    #region Msg2
    
    [DataContract]
    public class RpcMsgWrp<T1, T2> : IGeneralResObj
    {
        [DataMember(Order = 1)]
        public T1 P1 { get; set; }
        
        [DataMember(Order = 2)]
        public T2 P2 { get; set; }
        
        [DataMember(Order = 20)]
        public string ErrorMsg { get; set; }
    }
    
    #endregion
    
    #region Msg3
    
    [DataContract]
    public class RpcMsgWrp<T1, T2, T3> : IGeneralResObj
    {
        [DataMember(Order = 1)]
        public T1 P1 { get; set; }
        
        [DataMember(Order = 2)]
        public T2 P2 { get; set; }
        
        [DataMember(Order = 3)]
        public T3 P3 { get; set; }
        
        [DataMember(Order = 20)]
        public string ErrorMsg { get; set; }
    }
    
    #endregion
    
    #region Msg4
    
    [DataContract]
    public class RpcMsgWrp<T1, T2, T3, T4> : IGeneralResObj
    {
        [DataMember(Order = 1)]
        public T1 P1 { get; set; }
        
        [DataMember(Order = 2)]
        public T2 P2 { get; set; }
        
        [DataMember(Order = 3)]
        public T3 P3 { get; set; }
        
        [DataMember(Order = 4)]
        public T4 P4 { get; set; }
        
        [DataMember(Order = 20)]
        public string ErrorMsg { get; set; }
    }
    
    #endregion
    
    #region Msg5
    
    [DataContract]
    public class RpcMsgWrp<T1, T2, T3, T4, T5> : IGeneralResObj
    {
        [DataMember(Order = 1)]
        public T1 P1 { get; set; }
        
        [DataMember(Order = 2)]
        public T2 P2 { get; set; }
        
        [DataMember(Order = 3)]
        public T3 P3 { get; set; }
        
        [DataMember(Order = 4)]
        public T4 P4 { get; set; }
        
        [DataMember(Order = 5)]
        public T5 P5 { get; set; }
        
        [DataMember(Order = 20)]
        public string ErrorMsg { get; set; }
    }
    
    #endregion
    
    #region Msg6
    
    [DataContract]
    public class RpcMsgWrp<T1, T2, T3, T4, T5, T6> : IGeneralResObj
    {
        [DataMember(Order = 1)]
        public T1 P1 { get; set; }
        
        [DataMember(Order = 2)]
        public T2 P2 { get; set; }
        
        [DataMember(Order = 3)]
        public T3 P3 { get; set; }
        
        [DataMember(Order = 4)]
        public T4 P4 { get; set; }
        
        [DataMember(Order = 5)]
        public T5 P5 { get; set; }
        
        [DataMember(Order = 6)]
        public T6 P6 { get; set; }
        
        [DataMember(Order = 20)]
        public string ErrorMsg { get; set; }
    }
    
    #endregion
}
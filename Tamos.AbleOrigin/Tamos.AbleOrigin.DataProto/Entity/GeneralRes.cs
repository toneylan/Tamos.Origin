using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Tamos.AbleOrigin.DataProto
{
    [DataContract]
    public struct GeneralRes : IGeneralResObj
    {
        [DataMember(Order = 20)]
        public string? ErrorMsg { get; set; }

        /*/// <summary>
        /// Check error msg and join.
        /// </summary>
        public bool On(string? errorMsg)
        {
            if (errorMsg.IsNull()) return string.IsNullOrEmpty(ErrorMsg);

            if (ErrorMsg == null) ErrorMsg = errorMsg;
            else ErrorMsg += errorMsg;
            
            return string.IsNullOrEmpty(ErrorMsg);
        }*/
    }

    /// <summary>
    /// General res data wrapper
    /// </summary>
    [DataContract]
    public struct GeneralRes<T> : IGeneralResObj
    {
        [DataMember(Order = 1)]
        public T? Data { get; set; }

        [DataMember(Order = 20)]
        public string? ErrorMsg { get; set; }

        [MemberNotNullWhen(true, nameof(Data))]
        public bool IsSuccess()
        {
            return string.IsNullOrEmpty(ErrorMsg);
        }

        /// <summary>
        /// 将Data Map为TOut类型。
        /// </summary>
        public GeneralRes<TOut> To<TOut>() => new()
        {
            ErrorMsg = ErrorMsg,
            Data = Data == null ? default : EntMapper.Map<TOut>(Data)
        };
    }
}
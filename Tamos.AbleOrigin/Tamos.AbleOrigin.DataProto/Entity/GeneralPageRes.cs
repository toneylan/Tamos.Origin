using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Tamos.AbleOrigin.DataProto;

[DataContract]
public class GeneralPageRes<T> : IGeneralResObj
{
    [DataMember(Order = 1)]
    public List<T>? Data { get; set; }

    [DataMember(Order = 2)]
    public int Total { get; set; }

    [DataMember(Order = 20)]
    public string? ErrorMsg { get; set; }

    [MemberNotNullWhen(true, nameof(Data))]
    public bool IsSuccess()
    {
        return string.IsNullOrEmpty(ErrorMsg);
    }

    /// <summary>
    /// Map to other result type.
    /// </summary>
    public GeneralPageRes<TRes> Map<TRes>()
    {
        return new()
        {
            Total = Total,
            ErrorMsg = ErrorMsg,
            Data = EntMapper.Map<List<TRes>>(Data)
        };
    }
}
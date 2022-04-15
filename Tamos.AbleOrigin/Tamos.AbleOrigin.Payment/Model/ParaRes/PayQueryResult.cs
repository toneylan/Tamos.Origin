using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.Payment
{
    public class PayQueryResult : IGeneralResObj
    {
        public PayProcessResType ResultType { get; set; }

        public string? ErrorMsg { get; set; }
    }
}
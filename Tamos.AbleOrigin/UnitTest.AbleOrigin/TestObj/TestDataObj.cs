using System.Runtime.Serialization;
using ProtoBuf;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.UnitTest
{
    [DataContract]
    public class TestDataObj
    {
        public long Id { get; set; }
        public string Name { get; set; }

        [ProtoMember(1)]
        public Size Size { get; set; }
    }
}
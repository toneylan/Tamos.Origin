using System.ComponentModel;

namespace Tamos.AbleOrigin.DataProto
{
    public enum Gender
    {
        [Description("未知")]
        Unknown = 0,
        
        [Description("男")]
        Male = 1,
        
        [Description("女")]
        Female = 2
    }
}
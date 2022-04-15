using System.ComponentModel;

namespace Tamos.AbleOrigin.DataProto
{
    /// <summary>
    /// 身份证件类型
    /// </summary>
    public enum IdDocType
    {
        [Description("身份证")]
        CnId = 0,

        [Description("护照")]
        Passport = 1
    }
}
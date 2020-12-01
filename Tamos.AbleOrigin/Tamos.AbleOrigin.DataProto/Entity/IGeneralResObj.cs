namespace Tamos.AbleOrigin.DataProto
{
    /// <summary>
    /// General 响应/调用结果对象
    /// </summary>
    public interface IGeneralResObj
    {
        /// <summary>
        /// 错误提示信息
        /// </summary>
        string ErrorMsg { get; set; }
    }
}
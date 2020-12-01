namespace Tamos.AbleOrigin.DataProto
{
    /// <summary>
    /// 分页查询参数接口
    /// </summary>
    public interface IPagingQueryPara
    {
        /// <summary>
        /// 页索引
        /// </summary>
        int PageIndex { get; }

        /// <summary>
        /// 每页大小
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// 是否查询总记录数
        /// </summary>
        bool QueryTotal { get; }

        /// <summary>
        /// 总记录数
        /// </summary>
        int Count { get; set; }
    }
}
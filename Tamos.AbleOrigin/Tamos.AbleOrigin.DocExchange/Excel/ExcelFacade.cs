namespace Tamos.AbleOrigin.DocExchange.Excel;

/// <summary>
/// Excel文档的功能入口
/// </summary>
public static class ExcelFacade
{
    #region Version check

    /// <summary>
    /// 是否Excel2007版本。
    /// </summary>
    internal static bool IsXSSF(string filePath)
    {
        return filePath.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region Extensions
    

    #endregion
}
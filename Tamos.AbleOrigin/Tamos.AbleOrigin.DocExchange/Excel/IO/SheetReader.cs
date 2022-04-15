using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Tamos.AbleOrigin.DocExchange.Excel;

/// <summary>
/// Excel Sheet读取
/// </summary>
public class SheetReader : IDisposable
{
    protected ISheet Sheet { get; private set; }
    protected IRow? CurRow { get; set; }

    #region Create

    /// <summary>
    /// 从Excel文件创建Reader。
    /// </summary>
    public SheetReader(string filePath)
    {
        //if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return null;

        using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        IWorkbook workbook = ExcelFacade.IsXSSF(filePath)
            ? new XSSFWorkbook(stream)
            : new HSSFWorkbook(stream);

        Sheet = workbook.GetSheetAt(0);
    }

    #endregion

    #region Locate Nav

    /// <summary>
    /// 获取或设置所在行索引
    /// </summary>
    public int RowIndex
    {
        get => CurRow?.RowNum ?? -1;
        set
        {
            if (value < 0)
            {
                CurRow = null;
                return;
            }
            CurRow = Sheet.GetRow(value);
        }
    }

    /// <summary>
    /// 转到下一行，如果已在最后一行，返回false。
    /// </summary>
    public bool NextRow()
    {
        RowIndex += 1;

        return CurRow != null;
    }

    /// <summary>
    /// 转到下一个Sheet
    /// </summary>
    public bool NextSheet()
    {
        var workBook = Sheet.Workbook;
        var curSheetIndex = workBook.GetSheetIndex(Sheet);
        if (curSheetIndex >= workBook.NumberOfSheets - 1) return false;

        Sheet = workBook.GetSheetAt(curSheetIndex + 1);
        RowIndex = -1;
        return true;
    }

    #endregion

    #region Read value

    /// <summary>
    /// 获取Cell字符串值
    /// </summary>
    public string? CellValue(int columnIndex, string? defaultValue = null)
    {
        var strValue = CurRow?.GetCell(columnIndex)?.ToString();
        return strValue ?? defaultValue;
    }

    /// <summary>
    /// 获取Cell int值
    /// </summary>
    public int CellIntValue(int columnIndex, int defaultValue = 0)
    {
        return CellValue(columnIndex).ToInt(defaultValue);
    }

    /// <summary>
    /// 获取Cell decimal值
    /// </summary>
    public decimal CellDecimalValue(int columnIndex, decimal defaultValue = 0)
    {
        var strValue = CellValue(columnIndex);
        return decimal.TryParse(strValue, out var res) ? res : defaultValue;
    }

    #endregion

    public void Dispose()
    {
        Sheet.Workbook.Close();
    }
}
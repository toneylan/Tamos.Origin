using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace Tamos.AbleOrigin.DocExchange.Excel;

/// <summary>
/// Sheet内容Writer。
/// </summary>
public class SheetWriter
{
    protected ISheet Sheet { get; }
    protected IRow? CurRow { get; set; }

    /// <summary>
    /// 当前写入列
    /// </summary>
    public int ColumnIndex { get; set; }

    /// <summary>
    /// 当前写入行
    /// </summary>
    public int RowIndex
    {
        get => CurRow?.RowNum ?? -1;
        set
        {
            if (value < 0 || CurRow?.RowNum == value) return;
            CurRow = Sheet.GetRow(value) ?? Sheet.CreateRow(value);
            ColumnIndex = 0;
        }
    }

    internal SheetWriter(ISheet sheet)
    {
        Sheet = sheet;
        RowIndex = 0;
    }

    #region Layout set

    /// <summary>
    /// 新建一行
    /// </summary>
    public void NewRow()
    {
        RowIndex += 1;
    }

    /// <summary>
    /// 合并单元格：起始行号，终止行号， 起始列号，终止列号
    /// </summary>
    public void MergedRegion(int startRow, int endRow, int startCol, int endCol)
    {
        var range = new CellRangeAddress(startRow, endRow, startCol, endCol);
        Sheet.AddMergedRegion(range);
        //修复样式，是在第一个单元格样式进行合并
        /*var mergeCell = (CurRow?.RowNum == startRow ? CurRow : Sheet.GetRow(startRow))?.GetCell(startCol);
        if (mergeCell != null) ExcelFacade.SetMergeCellStyle(mergeCell, range);*/
    }

    /// <summary>
    /// 设置行高
    /// </summary>
    public void SetRowHeight(int height)
    {
        if (CurRow == null) return;
        CurRow.HeightInPoints = height;
    }

    /// <summary>
    /// 设置行内Cell样式
    /// </summary>
    public SheetWriter SetStyle(Action<LayoutStyle> setAct, int? colIndex = null)
    {
        var style = new LayoutStyle();
        setAct(style);
        style.Build(Sheet.Workbook);

        GetOrAddCell(colIndex ?? ColumnIndex).CellStyle = style.InnerStyle;
        return this;
    }

    #endregion

    #region Write cell

    /// <summary>
    /// 写入Cell值，并后移ColumnIndex。
    /// </summary>
    public SheetWriter Write(string? value)
    {
        GetOrAddCell(ColumnIndex++).SetCellValue(value);
        return this;
    }

    /// <summary>
    /// 写入数值类型，并后移ColumnIndex。
    /// </summary>
    public SheetWriter Write(double value)
    {
        GetOrAddCell(ColumnIndex++).SetCellValue(value);
        return this;
    }

    /// <summary>
    /// 写入多个Cell数据。
    /// </summary>
    public SheetWriter Write(params string?[] values)
    {
        foreach (var value in values)
        {
            Write(value);
        }
        return this;
    }

    #endregion

    #region Cell util
    
    private ICell GetOrAddCell(int colIndex)
    {
        return CurRow!.GetCell(colIndex) ?? CurRow.CreateCell(colIndex); //CurRow.CreateCell(ColumnIndex, type)
    }

    #endregion
}
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Tamos.AbleOrigin.DocExchange.Excel;

/// <summary>
/// 表示一页Sheet表格
/// </summary>
public class SheetPage
{
    private readonly IWorkbook Workbook;

    internal ISheet Sheet { get; set; }

    public SheetWriter Writer { get; }

    /// <summary>
    /// Excel里SheetName不允许“[]\/?”等特殊字符
    /// </summary>
    public SheetPage(string? sheetName)
    {
        Workbook = new XSSFWorkbook();
        Sheet = Workbook.CreateSheet(sheetName ?? "Sheet" + (Workbook.NumberOfSheets + 1));
        Writer = new SheetWriter(Sheet);
    }

    #region Write content

    /// <summary>
    /// 将列表按行写入
    /// </summary>
    public SheetPage WriteData<T>(IReadOnlyCollection<T>? list, Action<T> writeRow)
    {
        if (list.IsNull()) return this;

        foreach (var item in list)
        {
            writeRow(item);
            Writer.NewRow();
        }
        return this;
    }

    /// <summary>
    /// 写入一行内容
    /// </summary>
    public SheetPage WriteRow(params string[] cellValues)
    {
        foreach (var value in cellValues)
        {
            Writer.Write(value);
        }
        Writer.NewRow();
        return this;
    }

    /*private static void ApplyCellValueAndLayout(ICell cell, SheetCell celObj, int curRow, int curColumn, int sectionColumnCount)
    {
        //set cell value
        if (!string.IsNullOrEmpty(celObj.Value)) cell.SetCellValue(celObj.Value);

        //call layout set
        var layoutSet = celObj.LayoutSet;
        CellRangeAddress mergeRange = null;
        if (layoutSet != null)
        {
            if (layoutSet.RowSpan > 1 || layoutSet.ColumnSpan > 1 || layoutSet.HorizontalStretch)
            {
                var lastRow = layoutSet.RowSpan > 1 ? curRow + layoutSet.RowSpan - 1 : curRow;
                var lastColumn = layoutSet.HorizontalStretch
                    ? curColumn + sectionColumnCount - 1
                    : (layoutSet.ColumnSpan > 1 ? curColumn + layoutSet.ColumnSpan - 1 : curColumn);
                if (curRow != lastRow || curColumn != lastColumn)
                {
                    mergeRange = new CellRangeAddress(curRow, lastRow, curColumn, lastColumn);
                    cell.Sheet.AddMergedRegion(mergeRange);
                }
            }
            if (layoutSet.ColumnWidth > 0)
            {
                cell.Sheet.SetColumnWidth(cell.ColumnIndex, celObj.LayoutSet.ColumnWidth * 256);
            }
            if (layoutSet.RowHeight > 0)
            {
                cell.Row.HeightInPoints = layoutSet.RowHeight;
            }
        }

        //cell style set
        var styleSet = celObj.LayoutStyle;
        if (styleSet != null)
        {
            cell.CellStyle = cell.Sheet.Workbook.GetCellStyleAt(styleSet.StoreIndex);
            if (styleSet.ApplyToColumn && cell.CellStyle != null)
            {
                var aplStyle = cell.CellStyle;
                if (styleSet.HasBorder || !string.IsNullOrEmpty(styleSet.BackgroundColor)) //边框和颜色不应用到列
                {
                    aplStyle = cell.Sheet.Workbook.CreateCellStyle();
                    aplStyle.CloneStyleFrom(cell.CellStyle);
                    aplStyle.FillPattern = FillPattern.NoFill;
                    aplStyle.BorderLeft = aplStyle.BorderTop = aplStyle.BorderRight = aplStyle.BorderBottom = BorderStyle.None;
                }
                cell.Sheet.SetDefaultColumnStyle(cell.ColumnIndex, aplStyle);
            }

            //修复合并单元格样式
            if (mergeRange != null) SetMergeCellStyle(cell, mergeRange);
        }
    }*/

    /*private static void WriteHeaderOrFooter(List<SheetRow> rows, ISheet sheet, ref int curRowIndex, int secColumnCount)
    {
        foreach (var header in rows)
        {
            var curColumnIndex = 0;
            var row = sheet.CreateRow(curRowIndex);
            //loop header cell
            foreach (var celObj in header.Cells)
            {
                if (celObj.ChildrenCount < 1)
                {
                    //single cell
                    var cell = row.CreateCell(curColumnIndex);
                    ApplyCellValueAndLayout(cell, celObj, curRowIndex, curColumnIndex, secColumnCount);
                }
                else
                {
                    //with children cells
                    int rowOffset = 0, columnOffset = 0;
                    IRow tpRow = null;
                    foreach (var subCel in celObj.ChildrenCells)
                    {
                        var celRowNum = curRowIndex + rowOffset;
                        if (tpRow == null) //set row of subCell
                        {
                            if (rowOffset == 0) tpRow = row;
                            else tpRow = sheet.GetRow(celRowNum) ?? sheet.CreateRow(celRowNum);
                        }

                        var cell = tpRow.CreateCell(curColumnIndex + columnOffset);
                        ApplyCellValueAndLayout(cell, subCel, celRowNum, curColumnIndex + columnOffset, secColumnCount);

                        //check offset
                        if (columnOffset + subCel.ColumnSpanOrDefault >= celObj.ColumnSpanOrDefault) //new row
                        {
                            rowOffset++;
                            columnOffset = 0;
                            tpRow = null;
                        }
                        else
                        {
                            columnOffset += subCel.ColumnSpanOrDefault;
                        }
                    }
                }
                curColumnIndex += celObj.ColumnSpanOrDefault;
            }
            curRowIndex += header.RowSpan;
        }
    }*/

    #endregion

    #region 导出

    /// <summary>
    /// 保存到文件
    /// </summary>
    public void SaveFile(string path)
    {
        if (!ExcelFacade.IsXSSF(path)) path += ".xlsx";

        //检查目录
        FileUtil.EnsureDirExists(path);

        using var stream = new FileStream(path, FileMode.Create);
        Workbook.Write(stream);
    }

    #endregion
}
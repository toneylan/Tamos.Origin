using System.Drawing;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Tamos.AbleOrigin.DocExchange.Excel;

public class LayoutStyle
{
    internal ICellStyle InnerStyle { get; set; }

    /*/// <summary>
    /// 样式解析到Workbook中存储索引
    /// </summary>
    internal short StoreIndex { get; set; }*/

    /// <summary>
    /// 字号
    /// </summary>
    public short FontPoints { get; set; }

    /// <summary>
    /// 字体是否加粗
    /// </summary>
    public bool FontBold { get; set; }

    /// <summary>
    /// 水平对其方式
    /// </summary>
    public AlignmentMode Alignment { get; set; }

    /// <summary>
    /// 垂直对其方式
    /// </summary>
    public AlignmentMode VerticalAlign { get; set; }

    /// <summary>
    /// 背景颜色
    /// </summary>
    public Color? BackgroundColor { get; set; }

    /// <summary>
    /// 是否有边框
    /// </summary>
    public bool HasBorder { get; set; }

    /*/// <summary>
    /// 是否把当前样式设置到列的默认样式
    /// </summary>
    public bool ApplyToColumn { get; set; }*/

    internal LayoutStyle()
    {
    }

    #region Build style

    internal void Build(IWorkbook workbook)
    {
        InnerStyle = workbook.CreateCellStyle();

        //字体
        if (FontPoints > 0 || FontBold)
        {
            var font = workbook.CreateFont();
            if (FontPoints > 0) font.FontHeightInPoints = FontPoints;
            if (FontBold) font.IsBold = true;
            InnerStyle.SetFont(font);
        }

        //对齐
        if (Alignment != AlignmentMode.NotSet)
        {
            InnerStyle.Alignment = Alignment switch
            {
                AlignmentMode.Center => HorizontalAlignment.Center,
                AlignmentMode.Left => HorizontalAlignment.Left,
                AlignmentMode.Right => HorizontalAlignment.Right,
                _ => default
            };
        }
        if (VerticalAlign != AlignmentMode.NotSet)
        {
            InnerStyle.VerticalAlignment = VerticalAlign switch
            {
                AlignmentMode.Center => VerticalAlignment.Center,
                AlignmentMode.Top => VerticalAlignment.Top,
                AlignmentMode.Bottom => VerticalAlignment.Bottom,
                _ => default
            };
        }

        //set bg color
        if (BackgroundColor != null)
        {
            //设置FillForegroundColor，背景色才生效。
            InnerStyle.FillPattern = FillPattern.SolidForeground;
            (InnerStyle as XSSFCellStyle)?.SetFillForegroundColor(new XSSFColor(BackgroundColor.Value));
        }

        //set border
        if (HasBorder)
        {
            InnerStyle.BorderLeft = InnerStyle.BorderTop = InnerStyle.BorderRight = InnerStyle.BorderBottom = BorderStyle.Thin;
        }
    }
    
    //NPOI 合并单元格后，边框会有缺失，这里执行恢复设置
    /*internal static void SetMergeCellStyle(ICell mergeCell, CellRangeAddress range)
    {
        var style = mergeCell.CellStyle;
        var sheet = mergeCell.Sheet;
        for (var ri = range.FirstRow; ri <= range.LastRow; ri++)
        {
            var row = sheet.GetRow(ri) ?? sheet.CreateRow(ri);
            for (var ci = range.FirstColumn; ci <= range.LastColumn; ci++)
            {
                var cell = row.GetCell(ci) ?? row.CreateCell(ci, mergeCell.CellType);
                cell.CellStyle = style;
            }
        }
    }*/

    #endregion
}
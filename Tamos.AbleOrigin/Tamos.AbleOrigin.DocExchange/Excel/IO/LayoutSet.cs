namespace Tamos.AbleOrigin.DocExchange.Excel;

public class LayoutSet
{
    /// <summary>
    /// 跨几行
    /// </summary>
    public int RowSpan { get; set; }

    /// <summary>
    /// 跨几列
    /// </summary>
    public int ColumnSpan { get; set; }

    /// <summary>
    /// 水平占满SheetSection列宽度
    /// </summary>
    public bool HorizontalStretch { get; set; }

    /// <summary>
    /// 设置所在列的宽度(字符宽度)
    /// </summary>
    public int ColumnWidth { get; set; }

    /// <summary>
    /// 设置所在行的高度(一行中有一个设置即可)
    /// </summary>
    public int RowHeight { get; set; }
}

public enum AlignmentMode
{
    NotSet = 0,
    Left,
    Right,
    Center,
    Top,
    Bottom
}
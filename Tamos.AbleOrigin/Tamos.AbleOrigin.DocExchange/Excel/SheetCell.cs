namespace Tamos.AbleOrigin.DocExchange.Excel;

/// <summary>
/// 表示基础单元格
/// </summary>
public class SheetCell
{
    private List<SheetCell>? _childrens;

    public string Value { get; set; }
    public LayoutSet? LayoutSet { get; set; }
    public LayoutStyle? LayoutStyle { get; set; }
    public IEnumerable<SheetCell>? ChildrenCells => _childrens;

    public SheetCell()
    {
			
    }
    public SheetCell(string value, LayoutSet layoutSet = null, LayoutStyle style = null)
    {
        Value = value;
        LayoutStyle = style;
        LayoutSet = layoutSet;
    }

    public SheetCell(string value, int columnWidth = 0, LayoutStyle style = null)
    {
        Value = value;
        LayoutStyle = style;
        if (columnWidth > 0) LayoutSet = new LayoutSet {ColumnWidth = columnWidth};
    }

    public SheetCell(IEnumerable<SheetCell> childrenCells)
    {
        AddCell(childrenCells.ToArray());
    }

    public int ChildrenCount => _childrens?.Count ?? 0;

    public int ColumnSpanOrDefault => LayoutSet == null ? 1 : (LayoutSet.ColumnSpan > 1 ? LayoutSet.ColumnSpan : 1);

    public int RowSpanOrDefault => LayoutSet == null ? 1 : (LayoutSet.RowSpan > 1 ? LayoutSet.RowSpan : 1);

    #region Add a cell, recheck the row and column span.

    public void AddCell(params SheetCell[] cells)
    {
        if (_childrens == null)
        {
            _childrens = new List<SheetCell>();
        }
        _childrens.AddRange(cells);
        if (LayoutSet == null) LayoutSet = new LayoutSet { RowSpan = 1, ColumnSpan = 1 };

        //check column count
        foreach (var cell in cells)
        {
            if (cell.LayoutSet == null) continue;
            if (cell.LayoutSet.HorizontalStretch) LayoutSet.HorizontalStretch = true;
            if (cell.LayoutSet.ColumnSpan > LayoutSet.ColumnSpan) LayoutSet.ColumnSpan = cell.LayoutSet.ColumnSpan;
        }
        //check row count
        LayoutSet.RowSpan = _childrens.Sum(x => x.ColumnSpanOrDefault)/LayoutSet.ColumnSpan;
    }

    #endregion
}
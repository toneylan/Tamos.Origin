namespace Tamos.AbleOrigin.DocExchange.Excel;

/// <summary>
/// Sheet的一行
/// </summary>
public class SheetRow
{
    private readonly List<SheetCell> _cellList;

    public IEnumerable<SheetCell> Cells { get; private set; }
    
    public SheetRow(IEnumerable<SheetCell> cells)
    {
        _cellList = new List<SheetCell>();
        if (cells != null) _cellList.AddRange(cells);
        Cells = _cellList;
    }

    public void AddCell(SheetCell cell)
    {
        _cellList.Add(cell);
    }

    /*public void AddCell(string value, LayoutStyle style)
    {
        _cellList.Add(new BaseCell
        {
            Value = value,
            LayoutSet = style
        });
    }*/

    public int ColumnCount
    {
        get { return _cellList.Sum(x => x.ColumnSpanOrDefault); }
    }

    public int RowSpan
    {
        get { return _cellList.Max(x => x.RowSpanOrDefault); }
    }
}
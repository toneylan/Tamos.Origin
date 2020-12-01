using System.Collections.Generic;
using System.Linq;

namespace Tamos.AbleOrigin.DocExchange.ExcelKit
{
	/// <summary>
	/// Sheet的一行
	/// </summary>
	public class SheetRow
	{
		private readonly List<BaseCell> _cellList;

		public IEnumerable<BaseCell> Cells { get; private set; }

		public SheetRow() : this(null)
		{
		}

		public SheetRow(IEnumerable<BaseCell> cells)
		{
			_cellList = new List<BaseCell>();
			if (cells != null) _cellList.AddRange(cells);
			Cells = _cellList;
		}

		public void AddCell(BaseCell cell)
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
}
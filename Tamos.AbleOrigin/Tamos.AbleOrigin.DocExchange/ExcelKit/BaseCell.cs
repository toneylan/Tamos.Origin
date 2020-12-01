using System.Collections.Generic;
using System.Linq;

namespace Tamos.AbleOrigin.DocExchange.ExcelKit
{
	/// <summary>
	/// 表示基础单元格
	/// </summary>
	public class BaseCell
	{
		private List<BaseCell> _childrens;

		public string Value { get; set; }
		public LayoutSet LayoutSet { get; set; }
		public LayoutStyle LayoutStyle { get; set; }
	    public IEnumerable<BaseCell> ChildrenCells => _childrens;

		public BaseCell()
		{
			
		}
		public BaseCell(string value, LayoutSet layoutSet = null, LayoutStyle style = null)
		{
			Value = value;
			LayoutStyle = style;
			LayoutSet = layoutSet;
		}

		public BaseCell(string value, int columnWidth = 0, LayoutStyle style = null)
		{
			Value = value;
			LayoutStyle = style;
			if (columnWidth > 0) LayoutSet = new LayoutSet {ColumnWidth = columnWidth};
		}

		public BaseCell(IEnumerable<BaseCell> childrenCells)
		{
			AddCell(childrenCells.ToArray());
		}

		public int ChildrenCount
		{
			get { return _childrens == null ? 0 : _childrens.Count; }
		}

		public int ColumnSpanOrDefault
		{
			get { return LayoutSet == null ? 1 : (LayoutSet.ColumnSpan > 1 ? LayoutSet.ColumnSpan : 1); }
		}

		public int RowSpanOrDefault
		{
			get { return LayoutSet == null ? 1 : (LayoutSet.RowSpan > 1 ? LayoutSet.RowSpan : 1); }
		}

		#region Add a cell, recheck the row and column span.

		public void AddCell(params BaseCell[] cells)
		{
			if (_childrens == null)
			{
				_childrens = new List<BaseCell>();
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
}
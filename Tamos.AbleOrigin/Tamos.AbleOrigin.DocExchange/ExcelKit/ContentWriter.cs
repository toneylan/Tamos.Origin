using System;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace Tamos.AbleOrigin.DocExchange.ExcelKit
{
    #region IContentWriter

    /// <summary>
    /// 内容输出器，可交给调用方即时写入内容。
    /// 减少将数据装换成中间对象或数据时的内存浪费，也可在大数据量时分批读取数据源进行写入。
    /// </summary>
    public interface IContentWriter
    {
        /// <summary>
        /// 当前在Sheet中的写入行
        /// </summary>
        int CurRowIndex { get; set; }

        /// <summary>
        /// 当前在Sheet中的写入列
        /// </summary>
        int CurColumnIndex { get; set; }

        /// <summary>
        /// 新创建一行
        /// </summary>
        void NewRow();

        /// <summary>
        /// 合并单元格：起始行号，终止行号， 起始列号，终止列号
        /// </summary>
        void MergedRegion(int startRow, int endRow, int startCol, int endCol);

        /// <summary>
        /// 设置行高
        /// </summary>
        void SetRowHeight(int height);

        /// <summary>
        /// 渐进式的写入Cell值(即调用后CurColumnIndex增加1)
        /// </summary>
        void WriteCell(string value, LayoutStyle style = null);

        /// <summary>
        /// 写入int类型
        /// </summary>
        void WriteCell(int value, LayoutStyle style = null, bool zeroEmpty = false);

        /// <summary>
        /// 写入数值类型
        /// </summary>
        void WriteCell(double value, LayoutStyle style = null, bool zeroEmpty = false);

        /// <summary>
        /// 写入数值类型
        /// </summary>
        void WriteCell(decimal value, LayoutStyle style = null, bool zeroEmpty = false);
    }

    #endregion

    public class NPOIContentWriter : IContentWriter
    {
        protected ISheet Sheet { get; }
        protected IRow CurRow { get; set; }

        public int CurColumnIndex { get; set; }

        public int CurRowIndex
        {
            get { return CurRow?.RowNum ?? -1; }
            set
            {
                if (value < 0) return;
                if (CurRow == null)
                {
                    CurRow = Sheet.CreateRow(value);
                    CurColumnIndex = 0;
                }
                else if (CurRow.RowNum != value)
                {
                    CurRow = Sheet.GetRow(value) ?? Sheet.CreateRow(value);
                    CurColumnIndex = 0;
                }
            }
        }

        internal NPOIContentWriter(ISheet sheet, int curRow)
        {
            if (sheet == null) throw new ArgumentNullException();
            Sheet = sheet;
            CurRowIndex = curRow;
        }

        public void NewRow()
        {
            CurRowIndex += 1;
            //if (height != null && CurRow != null) CurRow.HeightInPoints = height.Value;
        }

        /// <summary>
        /// 合并单元格：起始行号，终止行号， 起始列号，终止列号
        /// </summary>
        public void MergedRegion(int startRow, int endRow, int startCol, int endCol)
        {
            var range = new CellRangeAddress(startRow, endRow, startCol, endCol);
            Sheet.AddMergedRegion(range);
            //修复样式，假定是在第一个单元格样式执行合并
            var mergeCell = (CurRow?.RowNum == startRow ? CurRow : Sheet.GetRow(startRow))?.GetCell(startCol);
            if (mergeCell != null) ExcelAdapter.SetMergeCellStyle(mergeCell, range);
        }

        public void SetRowHeight(int height)
        {
            if (CurRow == null) return;
            CurRow.HeightInPoints = height;
        }

        public void WriteCell(string value, LayoutStyle style = null)
        {
            FetchCurCell(CellType.String, style)?.SetCellValue(value);
        }

        public void WriteCell(double value, LayoutStyle style = null, bool zeroEmpty = false)
        {
            SetNumCell(value, style, zeroEmpty);
        }

        public void WriteCell(int value, LayoutStyle style = null, bool zeroEmpty = false)
        {
            SetNumCell(value, style, zeroEmpty);
        }

        public void WriteCell(decimal value, LayoutStyle style = null, bool zeroEmpty = false)
        {
            SetNumCell((double)value, style, zeroEmpty);
        }
        
        #region Cell util

        private void SetNumCell(double value, LayoutStyle style, bool zeroEmpty)
        {
            if (zeroEmpty && Math.Abs(value) < 0.000001) FetchCurCell(CellType.String, style)?.SetCellValue(string.Empty);
            else FetchCurCell(CellType.Numeric, style)?.SetCellValue(value);
        }

        private ICell FetchCurCell(CellType type, LayoutStyle style)
        {
            if (CurRow == null) return null;
            var cell = CurRow.GetCell(CurColumnIndex) ?? CurRow.CreateCell(CurColumnIndex, type);
            CurColumnIndex++; //移到下一格

            if (style != null) cell.CellStyle = Sheet.Workbook.GetCellStyleAt(style.StoreIndex);
            return cell;
        }

        #endregion
    }
}
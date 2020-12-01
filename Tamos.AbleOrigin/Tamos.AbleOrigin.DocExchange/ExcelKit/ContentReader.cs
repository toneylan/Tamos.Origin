using System;
using System.IO;
using NPOI.SS.UserModel;

namespace Tamos.AbleOrigin.DocExchange.ExcelKit
{
    /// <summary>
    /// Excel内容读取
    /// </summary>
    public interface IContentReader : IDisposable
    {
        /// <summary>
        /// 获取或设置当前在Sheet中的行索引
        /// </summary>
        int CurRowIndex { get; set; }

        /// <summary>
        /// 跳到下一行，如果已到Excel内容的最后一行，则返回false
        /// </summary>
        bool NextRow();

        /// <summary>
        /// 跳到下一个Sheet
        /// </summary>
        bool NextSheet();

        /// <summary>
        /// 获取单元格的字符串值
        /// </summary>
        string GetCellValue(int columnIndex, string defaultValue = null);

        /// <summary>
        /// 获取单元格的字符串值
        /// </summary>
        int GetCellIntValue(int columnIndex, int defaultValue = 0);

        /// <summary>
        /// 获取单元格的数字值
        /// </summary>
        double GetCellNumericValue(int columnIndex, double defaultValue = 0);

        /// <summary>
        /// 获取单元格的时间
        /// </summary>
        DateTime GetCellDateTimeValue(int columnIndex, DateTime defaultValue);
    }

    public class NPOIContentReader : IContentReader
    {
        protected ISheet Sheet { get; private set; }
        protected IRow CurRow { get; set; }
        private Stream _sourceStream;

        internal NPOIContentReader(ISheet sheet, Stream sourceStream)
        {
            if (sheet == null) throw new ArgumentNullException();
            Sheet = sheet;
            _sourceStream = sourceStream;
        }

        public int CurRowIndex
        {
            get { return CurRow != null ? CurRow.RowNum : -1; }
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

        public bool NextRow()
        {
            CurRowIndex += 1;

            return CurRow != null;
        }

        public bool NextSheet()
        {
            var workBook = Sheet.Workbook;
            var curSheetIndex = workBook.GetSheetIndex(Sheet);
            if (curSheetIndex >= workBook.NumberOfSheets - 1) return false;

            Sheet = workBook.GetSheetAt(curSheetIndex + 1);
            CurRowIndex = -1;
            return true;
        }

        public string GetCellValue(int columnIndex, string defaultValue = null)
        {
            if (CurRow == null) return defaultValue;
            var cell = CurRow.GetCell(columnIndex);
            if (cell == null) return defaultValue;

            var strValue = cell.ToString();
            return string.IsNullOrEmpty(strValue) ? defaultValue : strValue;
        }

        public int GetCellIntValue(int columnIndex, int defaultValue = 0)
        {
            if (CurRow == null) return defaultValue;
            var cell = CurRow.GetCell(columnIndex);
            if (cell == null) return defaultValue;

            var strValue = cell.ToString();
            int res;
            return int.TryParse(strValue, out res) ? res : defaultValue;
        }

        public double GetCellNumericValue(int columnIndex, double defaultValue = 0)
        {
            if (CurRow == null) return defaultValue;
            var cell = CurRow.GetCell(columnIndex);
            if (cell == null) return defaultValue;

            try
            {
                return cell.NumericCellValue;
            }
            catch
            {
                var strValue = cell.ToString();
                double res;
                return double.TryParse(strValue, out res) ? res : defaultValue;
            }
        }

        public DateTime GetCellDateTimeValue(int columnIndex, DateTime defaultValue)
        {
            if (CurRow == null) return defaultValue;
            var cell = CurRow.GetCell(columnIndex);
            if (cell == null) return defaultValue;

            try
            {
                return cell.DateCellValue;
            }
            catch
            {
                var strValue = cell.ToString();
                DateTime res;
                return DateTime.TryParse(strValue, out res) ? res : defaultValue;
            }
        }

        public void Dispose()
        {
            CurRow = null;
            Sheet = null;
            if (_sourceStream != null) _sourceStream.Dispose();
        }
    }
}
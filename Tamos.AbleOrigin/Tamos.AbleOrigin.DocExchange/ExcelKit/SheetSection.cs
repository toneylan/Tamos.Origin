using System;
using System.Collections.Generic;
using System.Linq;

namespace Tamos.AbleOrigin.DocExchange.ExcelKit
{
	/// <summary>
	/// Excel sheet 的一个部分/段
	/// </summary>
	public class SheetSection
	{
		/// <summary>
		/// 头部行列表
		/// </summary>
		public List<SheetRow> Headers { get; set; }

        /// <summary>
        /// Body输出方法
        /// </summary>
        public Action<IContentWriter> BodyWriteAction { get; set; }

        /// <summary>
        /// 尾部行列表
        /// </summary>
        public List<SheetRow> Footers { get; set; }

        public int ColumnCount
		{
			get
			{
				var count = 0;
			    if (Headers?.Count > 0) count = Headers.Max(x => x.ColumnCount);
				return count;
			}
		}
	}
}
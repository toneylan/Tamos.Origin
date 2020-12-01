using System.Collections.Generic;

namespace Tamos.AbleOrigin.DocExchange.ExcelKit
{
	public class SheetPage
	{
        /// <summary>
        /// Excel里SheetName不允许“[]\/?”等特殊字符
        /// </summary>
		public string SheetName { get; set; }
		public List<SheetSection> Sections { get; set; }

        /// <summary>
        /// 存储Sheet中的样式列表
        /// </summary>
		internal List<LayoutStyle> SheetStyles { get; private set; }

		public SheetPage()
		{
			Sections = new List<SheetSection>();
			SheetStyles = new List<LayoutStyle>();
		}

		#region Sheet 样式设置

		/*public LayoutStyle CreateStyle()
		{
			return CreateStyle(AlignmentMode.NotSet);
		}*/

	    /// <summary>
	    /// 创建一个样式。注意：样式尽可能的重用，以减少样式的定义数量。
	    /// </summary>
	    public LayoutStyle CreateStyle(bool hasBorder = false, AlignmentMode alignment = AlignmentMode.NotSet, bool fontBold = false, short fontPoints = 0)
	    {
	        var style = CreateStyle(alignment, AlignmentMode.Center, false, fontBold, fontPoints);
	        style.HasBorder = hasBorder;
	        return style;
	    }

	    public LayoutStyle CreateStyle(AlignmentMode alignment, AlignmentMode verticalAlignment = AlignmentMode.Center, bool applyToColumn = false,
	        bool fontBold = false, short fontPoints = 0, string backColor = null)
	    {
	        var style = new LayoutStyle
	        {
	            Alignment = alignment,
	            VerticalAlignment = verticalAlignment,
	            FontBold = fontBold,
	            FontPoints = fontPoints,
	            ApplyToColumn = applyToColumn,
                BackgroundColor = backColor,
                //HasBorder = true
	        };
	        SheetStyles.Add(style);
	        return style;
	    }

	    #endregion
	}
}
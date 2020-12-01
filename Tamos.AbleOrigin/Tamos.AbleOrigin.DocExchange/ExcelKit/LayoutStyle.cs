namespace Tamos.AbleOrigin.DocExchange.ExcelKit
{
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

    public class LayoutStyle
    {
        private string _backgroundColor;

        internal LayoutStyle()
        {
            //HasBorder = true;
        }

        /// <summary>
        /// 样式解析到Woorbook中存储索引
        /// </summary>
        internal short StoreIndex { get; set; }

        /// <summary>
        /// 是否把当前样式设置到列的默认样式
        /// </summary>
        public bool ApplyToColumn { get; set; }

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
        public AlignmentMode VerticalAlignment { get; set; }

        /// <summary>
        /// 背景颜色，设置后将默认带边框
        /// </summary>
        public string BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                //if (!string.IsNullOrEmpty(_backgroundColor)) HasBorder = true;
            }
        }

        /// <summary>
        /// 是否设置边框
        /// </summary>
        public bool HasBorder { get; set; }
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
}
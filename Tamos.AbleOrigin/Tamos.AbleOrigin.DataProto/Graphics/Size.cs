using System.Runtime.Serialization;

namespace Tamos.AbleOrigin.DataProto
{
    /// <summary>
    /// 表示宽高尺寸信息
    /// </summary>
    [DataContract]
    public struct Size
    {
        [DataMember(Order = 1)]
        public int Width { get; set; }
        
        [DataMember(Order = 2)]
        public int Height { get; set; }
        
        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        #region 尺寸计算

        /*/// <summary>
        /// 是否完全包含目标尺寸，即宽高都大于或等于目标尺寸。
        /// </summary>
        public bool Contains(int width, int height)
        {
            return Width >= width && Height >= height;
        }*/

        /// <summary>
        /// 将尺寸缩减至当前宽高比，只会缩减更多那个维度。
        /// </summary>
        public Size RatioFloor(int rawWidth, int rawHeight)
        {
            //var ratio = (double) Width / Height;
            return (double) rawWidth / rawHeight > (double) Width / Height
                ? new Size(rawHeight * Width / Height, rawHeight) //宽度更多
                : new Size(rawWidth, rawWidth * Height / Width);
        }

        #endregion

        public static implicit operator Size((int, int) wh)
        {
            return new Size(wh.Item1, wh.Item2);
        }
    }
}
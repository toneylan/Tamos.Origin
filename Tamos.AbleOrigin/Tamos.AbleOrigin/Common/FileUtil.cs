using System;

namespace Tamos.AbleOrigin
{
    public static class FileUtil
    {
        private static readonly string[] SizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        /// <summary>
        /// 描述文件大小: *K / *M / *G ……
        /// </summary>
        public static string FriendlyDesSize(long size, int decimalLength = 1, int kiloUnit = 1024)
        {
            if (size <= 0) return "0B";

            var mag = (int) Math.Max(0, Math.Log(size, kiloUnit));
            var adjustedSize = Math.Round(size / Math.Pow(kiloUnit, mag), decimalLength);
            return $"{adjustedSize} {SizeSuffixes[mag]}";
        }
    }
}
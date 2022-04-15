namespace Tamos.AbleOrigin.DataProto
{
    public static class ProtoEnumExtension
    {
        /// <summary>
        /// 判断是否符合当前证件类型
        /// </summary>
        public static bool Validate(this IdDocType idType, string? num, bool allowNull = true)
        {
            if (num.IsNull()) return allowNull;

            if (idType == IdDocType.CnId) return Utility.IsCnId(num);
            
            return true;
        }
    }
}
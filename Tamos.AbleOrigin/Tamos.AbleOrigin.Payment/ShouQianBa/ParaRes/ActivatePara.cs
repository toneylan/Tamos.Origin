namespace Tamos.AbleOrigin.Payment.ShouQianBa
{
    public class ActivatePara
    {
        /// <summary>
        /// [Y]应用ID
        /// </summary>
        public string app_id { get; set; }
                
        /// <summary>
        /// [Y]激活码内容
        /// </summary>
        public string code { get; set; }
                
        /// <summary>
        /// [Y(128)]内容自行定义，同一个app_id下唯一；为了方便识别，建议具有一定的格式；例：品牌名称+支付场景
        /// </summary>
        public string device_id { get; set; }
                
        /// <summary>
        /// [N(50)]第三方终端号，必须保证在app id下唯一
        /// </summary>
        public string client_sn { get; set; }
                
        /// <summary>
        /// [N(128)]终端名
        /// </summary>
        public string name { get; set; }
                
        /// <summary>
        /// [N(45)]当前系统信息，如: Android5.0
        /// </summary>
        public string os_info { get; set; }
                
        /// <summary>
        /// [N(45)]SDK版本
        /// </summary>
        public string sdk_version { get; set; }
    }
}
using System;

namespace Tamos.AbleOrigin.AspNetCore
{
    /// <summary>
    /// Web Api 自定义设置
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class OpenApiSettingAttribute : Attribute
    {
        /// <summary>
        /// 设置Api文档的tags标记，https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/README.md#customize-operation-tags-eg-for-ui-grouping
        /// OpenApi generator 也会用此Tag进行分组。
        /// </summary>
        public string Tag { get; set; }

        public OpenApiSettingAttribute(string tag)
        {
            Tag = tag;
        }
    }
}
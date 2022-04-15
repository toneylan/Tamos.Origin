using System;

namespace Tamos.AbleOrigin
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public class CodeGenSetAttribute : Attribute
    {
        /// <summary>
        /// 忽略当前接口或方法
        /// </summary>
        public bool Ignore { get; set; }

        /*/// <summary>
        /// 标记需要将父接口注册到IOC Proxy中
        /// </summary>
        public bool RegBaseImpl;*/
    }
}
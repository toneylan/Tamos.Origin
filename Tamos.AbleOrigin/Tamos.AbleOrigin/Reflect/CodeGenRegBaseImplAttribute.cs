using System;

namespace Tamos.AbleOrigin.Reflect
{
    /// <summary>
    /// 标记需要将父接口注册到IOC Proxy中
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class CodeGenRegBaseImplAttribute : Attribute
    {
        
    }
}
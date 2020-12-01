using System;
using Tamos.AbleOrigin.Common;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.MetaCoding.ReplanUI
{
    /// <summary>
    /// 呈现字段的基类
    /// </summary>
    internal abstract class BaseRenderField<T> where T : class
    {
        public string CnName { get; protected set; }

        /// <summary>
        /// 获取实例对象中的字段值
        /// </summary>
        public abstract string GetValue(T entity);
    }

    /// <summary>
    /// 属性构成的字段
    /// </summary>
    internal class PropertyField<TEnt> : BaseRenderField<TEnt> where TEnt : class
    {
        private readonly BasePropMeta<TEnt> _prop;
        
        //未设置的属性可能没有Meta信息
        public PropertyField(BasePropMeta<TEnt> prop)
        {
            _prop = prop;
            CnName = prop?.CnName;
        }

        public override string GetValue(TEnt entity)
        {
            var val = _prop?.GetValue(entity);
            if (val == null) return null;
            
            return val switch
            {
                DateTime time => time.ToString("yyyy-MM-dd HH:mm:ss"),
                Enum en => en.GetDes(),
                _ => val.ToString()
            };
        }
    }

    /// <summary>
    /// 自定义显示的字段
    /// </summary>
    internal class CustomField<T> : BaseRenderField<T> where T : class
    {
        private readonly Func<T, string> _valueGet;

        public CustomField(string cnName, Func<T, string> valueGet)
        {
            CnName = cnName;
            _valueGet = valueGet;
        }

        /// <summary>
        /// 自定义Get值
        /// </summary>
        public override string GetValue(T entity)
        {
            return _valueGet(entity);
        }
    }
}
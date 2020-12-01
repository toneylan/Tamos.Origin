using System.Diagnostics.CodeAnalysis;

namespace Tamos.MetaCoding.ReplanUI
{
    /// <summary>
    /// 开始Rui的配置
    /// </summary>
    public abstract class RuiConfig
    {
        #region Entity config set

        //private static readonly Dictionary<Type, RuiConfig> EntityConfigs = new Dictionary<Type, RuiConfig>();

        /// <summary>
        /// 注册Entity配置
        /// </summary>
        internal static RuiEntityConfig<T> Entity<T>() where T : class
        {
            var conf = new RuiEntityConfig<T>();
            //EntityConfigs.Add(typeof(T), conf);
            return conf;
        }

        /*internal static RuiEntityConfig<T> GetEntConfig<T>() where T : class
        {
            return EntityConfigs.GetValue(typeof(T)) as RuiEntityConfig<T>;
        }*/

        #endregion

        /*#region DefaultProp set

        private static Dictionary<string, string> _defaultPropSets;
        internal static Dictionary<string, string> DefaultPropSets => _defaultPropSets ??= new Dictionary<string, string>();

        /// <summary>
        /// 自定义一些默认的属性设置，减少每次的单独配置。
        /// </summary>
        public static void DefaultProp(params (string propName, string name)[] sets)
        {
            if (sets.IsNullOrEmpty()) return;

            foreach (var tp in sets)
            {
                DefaultPropSets.Add(tp.propName, tp.name);
            }
        }

        #endregion*/
    }

    /// <summary>
    /// Entity的Rui配置
    /// </summary>
    public class RuiEntityConfig<T> : RuiConfig where T : class
    {
        
        /*#region Col set

        /// <summary>
        /// 加入其他属性的配置。不包含：“_”打头的扩展属性、只读属性。
        /// </summary>
        public RuiEntityConfig<T> OtherPropExc(params string[] excludeProps)
        {
            foreach (var property in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty))
            {
                var propName = property.Name;
                if (propName.StartsWith('_') || excludeProps?.Contains(propName) == true) continue;

                //自定义默认配置
                if (DefaultPropSets.ContainsKey(propName))
                {
                    Prop(DefaultPropSets[propName], property);
                    continue;
                }
            }

            return this;
        }

        #endregion*/
    }
}
using Tamos.AbleOrigin.DataProto;

namespace Tamos.MetaCoding.ReplanUI
{
    public abstract class BaseRuiPlan<T> where T : class
    {
        protected EntityMeta<T> Meta { get; } = EntityMeta.Get<T>();

        /*internal static TPlan New<TPlan>(EntityMeta<T> meta) where TPlan : BaseRuiPlan<T>, new()
        {
            return new TPlan
            {
                Meta = meta
            };
        }*/

    }
}
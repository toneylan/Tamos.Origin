using System;

namespace Tamos.AbleOrigin.DataPersist
{
    public class YearScope : ContextScope
    {
        internal override ShardScopeType ScopeType => ShardScopeType.Year;
        
        internal override string? TableSuffix => OriginalPart ? null : StartDate.ToString("_yy");
        
        #region Create scope
        
        public YearScope(DateTime time)
        {
            if (time < ShardBeginTime)
            {
                OriginalPart = true;
                EndDate = ShardBeginTime.AddMilliseconds(-SpanMilliseconds);
                return;
            }

            //时间限度
            var shardEnd = MaxShardEndDate;
            if (time > shardEnd) time = shardEnd;

            //Scope时间范围
            StartDate = new DateTime(time.Year, 1, 1);
            EndDate = StartDate.AddYears(1).AddMilliseconds(-SpanMilliseconds);
        }
        
        #endregion
        
    }
}
using System;

namespace Tamos.AbleOrigin.DataPersist
{
    public class QuarterScope : ContextScope
    {
        internal override ShardScopeType ScopeType => ShardScopeType.Quarter;
        
        internal override string TableSuffix => OriginalPart ? string.Empty : StartDate.ToString("_yyMM");

        #region Create scope
        
        public QuarterScope(DateTime time)
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

            //计算所处季度
            var mod = time.Month%3;
            var month = time.Month;
            if (mod == 0) month -= 2;
            else if (mod == 2) month -= 1;

            StartDate = new DateTime(time.Year, month, 1);
            EndDate = StartDate.AddMonths(3).AddMilliseconds(-SpanMilliseconds);
        }

        #endregion
    }
}
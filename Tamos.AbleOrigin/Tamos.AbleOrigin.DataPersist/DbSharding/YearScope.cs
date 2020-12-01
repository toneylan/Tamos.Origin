using System;
using Tamos.AbleOrigin.Common;
using Tamos.AbleOrigin.Configuration;

namespace Tamos.AbleOrigin.DataPersist
{
    public class YearScope : ContextScope
    {
        //开始数据分片的时间，之前的数据会统一存在原始表中
        public static readonly DateTime ShardBeginTime = Utility.StrToDate(CentralConfiguration.Get("ExternalService/DbShardBeginTime"), new DateTime(2020, 1, 1));

        internal override ShardScopeType ScopeType => ShardScopeType.Year;
        
        internal override string TablePostfix => OriginalPart ? null : StartDate.ToString("_yy");
        
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
using System;
using System.Collections.Generic;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.DataPersist
{
    /// <summary>
    /// 表示ShardingDbContext的分表范围，比如时间、Id范围等
    /// </summary>
    public abstract class ContextScope
    {
        //开始数据分表的时间，之前的数据会存在原始表中
        internal static readonly DateTime ShardBeginTime = DataIdBuilder.ReferTime;

        protected const int SpanMilliseconds = 1;

        /// <summary>
        /// 原始未分表部分（归档）
        /// </summary>
        public bool OriginalPart { get; protected set; }
        
        /// <summary>
        /// 分表的范围类型
        /// </summary>
        internal abstract ShardScopeType ScopeType { get; }
        
        /// <summary>
        /// 分表后缀名
        /// </summary>
        internal abstract string TableSuffix { get; }

        #region 时间分表信息

        /// <summary>
        /// 最大的分表时间，防止创建太多未来时间的分表
        /// </summary>
        protected static DateTime MaxShardEndDate => DateTime.Today.AddYears(1);

        protected internal DateTime StartDate { get; protected set; }
        protected internal DateTime EndDate { get;  protected set;}

        /// <summary>
        /// 前一范围的结束时间
        /// </summary>
        protected internal DateTime PrevScopeEnd => StartDate > DateTime.MinValue ? StartDate.AddMilliseconds(-SpanMilliseconds) : StartDate;

        /// <summary>
        /// 是否包含了指定时间
        /// </summary>
        public virtual bool Contains(DateTime time)
        {
            return time >= StartDate && time <= EndDate;
        }

        #endregion

        #region Static Create scope

        internal static ContextScope Create(ShardScopeType scopeType, DateTime date)
        {
            switch (scopeType)
            {
                case ShardScopeType.Quarter:
                    return new QuarterScope(date);
                case ShardScopeType.Year:
                    return new YearScope(date);
                default:
                    throw new ArgumentOutOfRangeException(nameof(scopeType), scopeType, "未实现ScopeType的创建");
            }
        }

        internal static List<ContextScope> Create(ShardScopeType scopeType, DateTime start, DateTime end, bool avoidFutureTime)
        {
            var scopes = new List<ContextScope>();

            //时间检查，防止创建太多分表
            var tmpEnd = end;
            if (avoidFutureTime)
            {
                var now = DateTime.Now;
                tmpEnd = end > now ? now : end; //避免未来时间
            }
            else
            {
                var shardEnd = MaxShardEndDate; //分表时间上限
                if (tmpEnd > shardEnd) tmpEnd = shardEnd;
            }

            //时间倒序查找
            while (tmpEnd >= start)
            {
                var scp = Create(scopeType, tmpEnd);
                scopes.Add(scp);
                tmpEnd = scp.PrevScopeEnd;
            }

            return scopes;
        }
        
        #endregion
    }

    /// <summary>
    /// 分表范围的类型
    /// </summary>
    public enum ShardScopeType
    {
        /// <summary>
        /// 三个月
        /// </summary>
        Quarter = 1,

        /// <summary>
        /// 一年
        /// </summary>
        Year = 4
    }
}
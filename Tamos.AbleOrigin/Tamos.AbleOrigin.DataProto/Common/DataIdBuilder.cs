using System;
using System.Collections.Generic;
using Tamos.AbleOrigin.Common;

namespace Tamos.AbleOrigin.DataProto
{
    public static class DataIdBuilder
    {
        public static readonly DateTime ReferTime = new DateTime(2013, 1, 1); //重要！！不可修改，是订单编号中经过秒数的参照时间

        private static readonly Random NumRandom = new Random();
        private static readonly HashSet<long> CommonBuiltIds = new HashSet<long>(); //高频生成Id时，缓存记录以排重
        private static readonly HashSet<long> BuiltIncIds = new HashSet<long>(); //高频生成Id时，缓存记录以排重
        private static DateTime _lastGenerateTime;
        private static DateTime _incLastGenTime; //递增编号的最后生成时间
        private static int _incLastGenNum;

        #region GenerateId
        
        /// <summary>
        /// 通用的Id生成
        /// </summary>
        public static long GenerateId(long belongId, DateTime createTime)
        {
            if (createTime < ReferTime) createTime = new DateTime(ReferTime.Year, createTime.Month, createTime.Day).Add(createTime.TimeOfDay);

            return CheckGenerate(CommonBuiltIds, createTime, time => string.Format("{0}{1:0000}{2:00}{3}",
                (int) (time - ReferTime).TotalSeconds, NumRandom.Next(1, 9999), belongId % 100, 1).ToLong()); //scope 临时 1
        }
        
        //进行并发Id生成检查，能避免在进程级别生成相同Id（目前没分对象类别，比如Order、Ticket等全局生成不重复）
        private static long CheckGenerate(HashSet<long> dicBuilt, DateTime genTime, Func<DateTime, long> idBuilder, bool isIncId = false)
        {
            //生成一个Id
            var id = idBuilder(genTime);

            //生成间隔大于1秒，基本不会重复了
            if (Math.Abs((genTime - (isIncId ? _incLastGenTime : _lastGenerateTime)).TotalSeconds) > 1)
            {
                if (dicBuilt.Count > 100) dicBuilt.Clear(); //清理一下，备下次高频时缓存
            }
            else //---密集生成模式
            {
                var loopCount = 0;
                while (dicBuilt.Contains(id))
                {
                    if (++loopCount > 1000)
                    {
                        genTime = DateTime.Now; //若难以生成新Id，重取now以可能延迟到下一秒再尝试
                    }
                    id = idBuilder(genTime);
                }
            }

            //结果
            dicBuilt.Add(id);
            if (isIncId) _incLastGenTime = genTime;
            else _lastGenerateTime = genTime;
            return id;
        }

        /// <summary>
        /// 生成一个递增的编号
        /// </summary>
        public static long GenerateIncId(long belongId)
        {
            var now = DateTime.Now;
            //生成间隔大于一秒，重置递增量
            if ((now - _incLastGenTime).TotalSeconds > 1) _incLastGenNum = 0;

            //当前递增号，由于只保留2位，1秒内超过100后会从0开始
            var curNum = ++_incLastGenNum % 100;

            //两位递增+两位随机数，尽可能避免同尾号场馆在同一时刻的ID冲突
            return CheckGenerate(BuiltIncIds, now, time => string.Format("{0}{1:00}{2:00}{3:00}{4}",
                (int) (time - ReferTime).TotalSeconds, curNum, NumRandom.Next(1, 99), belongId % 100, 1).ToLong(), true);
        }

        #endregion

        #region Parse date
            
        /// <summary>
        /// 从Id解析出时间，支持16位格式
        /// </summary>
        public static DateTime ParseDate(long id)
        {
            var idStr = id.ToString();
            if (idStr.Length < 9) return DateTime.Today;

            return int.TryParse(idStr.Substring(0, 9), out var sec) ? ReferTime.AddSeconds(sec) : DateTime.Today;

            /*if (billId.Length < 16)
            {
                //（不足9位时间），日期较早如2015年前的
                int sec;
                return int.TryParse(billId.Substring(0, billId.Length - 7), out sec) ? OrderIdBuilder.ReferTime.AddSeconds(sec) : DateTime.Today;
            }*/
        }

        /// <summary>
        /// 解析时间范围
        /// </summary>
        public static (DateTime Start, DateTime End) ParseDateRange(IReadOnlyCollection<long> idList)
        {
            if (idList.IsNullOrEmpty()) return (DateTime.Today, DateTime.Today);

            DateTime minDate = DateTime.MaxValue, maxDate = DateTime.MinValue;
            foreach (var id in idList)
            {
                var date = ParseDate(id);
                if (date < minDate) minDate = date;
                if (date > maxDate) maxDate = date;
            }

            return (minDate, maxDate);
        }

        #endregion
    }
}
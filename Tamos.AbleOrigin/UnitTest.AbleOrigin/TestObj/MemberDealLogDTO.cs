using System;
using System.Net;

namespace UnitTest.AbleOrigin
{
    public class MemberDealLogDTO
    {
        public long TransactionId { get; set; }
        
        /// <summary>
        /// 关联的记录ID
        /// </summary>
        public long RelatedTranId { get; set; }
        
        /// <summary>
        /// 会员卡所属场馆
        /// </summary>
        public int MerchantId { get; set; }
        
        public int BrandId { get; set; }
        
        public long MemberId { get; set; }
        
        public long MemberCardId { get; set; }
        
        public DayOfWeek CardType { get; set; }
        
        /// <summary>
        /// 办理类型：购卡、补卡、暂停……
        /// </summary>
        public HttpResponseHeader DealType { get; set; }
        
        /// <summary>
        /// 收取金额
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 服务费，包含在TotalPrice中
        /// </summary>
        public decimal ServiceFee { get; set; }
        
        /// <summary>
        /// 调整的金额、天数（暂停、延期）等
        /// </summary>
        public decimal AdjustAmount { get; set; }
        
        public long OperClerkId { get; set; }
        
        public int OperMerchantId { get; set; }
        
        public long OperDepartmentId { get; set; }
        
        public DateTime CreateTime { get; set; }
        
        public DateTime LastUpdateTime { get; set; }
        
        public string Remark { get; set; }

        public static MemberDealLogDTO New()
        {
            return new MemberDealLogDTO
            {
                TransactionId = 1001,
                RelatedTranId = 111,
                MerchantId = 222,
                BrandId = 333,
                MemberId = 444,
                MemberCardId = 555,
                CardType = DayOfWeek.Sunday,
                DealType = HttpResponseHeader.CacheControl,
                TotalPrice = 66,
                ServiceFee = 0,
                AdjustAmount = 777,
                OperClerkId = 888,
                OperMerchantId = 999,
                OperDepartmentId = 100,
                CreateTime = DateTime.Now,
                LastUpdateTime = DateTime.Now,
                Remark = "test remark"
            };
        }
    }
}
using System;
using System.Net;
using NUnit.Framework;
using UnitTest.AbleOrigin;

namespace Tamos.AbleOrigin.UnitTest
{
    public class JsonSerializeTest
    {
        [Test]
        public void Perform()
        {
            var obj = new MemberDealLogDTO
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

            const int count = 50;
            Console.WriteLine($"Start test {count} times:");
            
            TestUtil.RunWatch("System.Text", () =>
            {
                for (var i = 0; i < count; i++)
                {
                    var str = System.Text.Json.JsonSerializer.Serialize(obj);
                    //if (i == 0) Console.WriteLine(str);
                }
            });

            TestUtil.RunWatch("Newtonsoft", () =>
            {
                for (var i = 0; i < count; i++)
                {
                    var str = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                    //if (i == 0) Console.WriteLine(str);
                }
            });
        }
    }
}
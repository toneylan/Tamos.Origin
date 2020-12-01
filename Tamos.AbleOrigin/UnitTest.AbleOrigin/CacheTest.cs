using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Tamos.AbleOrigin;
using Tamos.AbleOrigin.Booster;
using Tamos.AbleOrigin.Cache;
using Tamos.AbleOrigin.Serialize;
using Tamos.AbleOrigin.UnitTest;

namespace UnitTest.AbleOrigin
{
    public class CacheTest
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

            const int count = 2;
            Console.WriteLine($"Start test {count} times:");

            CacheService.Set("testData", obj, TimeSpan.FromDays(30));

            TestUtil.RunWatch("Get cache", () =>
            {
                for (var i = 0; i < count; i++)
                {
                    var res = CacheService.Get<MemberDealLogDTO>("testData");
                    Assert.NotNull(res);
                }
            });
        }

        [Test]
        public async Task DistEvent()
        {
            FrameInitializer.Initialize(null);

            const int count = 5;
            const string topic = "test_topic";

            DistEventService.Subscribe<TestDataObj>(topic, msg => Console.WriteLine($"Subscribe1 get:{SerializeUtil.ToJson(msg)}"));

            DistEventService.Subscribe<TestDataObj>(topic, msg => Console.WriteLine($"Subscribe2 get:{SerializeUtil.ToJson(msg)}"));
            
            //new TestDataObj {Id = 1001, Name = "testName"}

            await Task.Run(() =>
            {
                for (var i = 0; i < count; i++)
                {
                    DistEventService.Publish(topic, new TestDataObj {Id = 1000 + i, Name = DateTime.Now.ToShortTimeString()});

                    Thread.Sleep(3000);
                }
            });
        }
    }
}
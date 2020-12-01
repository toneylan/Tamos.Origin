using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using NUnit.Framework;
using Tamos.AbleOrigin.UnitTest;
using Tamos.MetaCoding.ReplanUI;

namespace UnitTest.AbleOrigin
{
    public class ReplanUITest
    {
        [Test]
        public void Perform()
        {
            var obj = MemberDealLogDTO.New();

            //Console.WriteLine($"Start test {count} times:");

            /*RuiConfig.Entity<MemberDealLogDTO>()
                .Prop("µ÷Õû½ð¶î", x => x.AdjustAmount)
                .OtherPropExc();

            var res = RuiFacade.ToListVm(new List<MemberDealLogDTO> {obj});

            Assert.NotNull(res);
            Console.WriteLine(JsonConvert.SerializeObject(res));*/
        }
    }
}
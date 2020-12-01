using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using NUnit.Framework;
using Tamos.MetaCoding.ReplanUI;

namespace UnitTest.AbleOrigin
{
    public class DbTest
    {
        [Test]
        public void DbMap()
        {
            var db = new LiveDbContext();
            var map = db.GetMap();

            Console.WriteLine(map);
        }
    }
}
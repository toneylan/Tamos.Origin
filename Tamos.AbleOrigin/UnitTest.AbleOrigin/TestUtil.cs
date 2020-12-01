using System;
using System.Diagnostics;

namespace Tamos.AbleOrigin.UnitTest
{
    public class TestUtil
    {
        public static void RunWatch(string title, Action action)
        {
            RunWatch(title, 0, action);
        }

        public static void RunWatch(string title, int loopTimes, Action action)
        {
            var watch = Stopwatch.StartNew();
            if (loopTimes <= 1) action();
            else
            {
                for (var i = 0; i < loopTimes; i++) action();
            }
            watch.Stop();

            Console.WriteLine(
                $"{title} use time: {(watch.ElapsedMilliseconds > 0 ? $"{watch.ElapsedMilliseconds} ms" : $"{watch.ElapsedTicks} tick")}");
        }
    }
}
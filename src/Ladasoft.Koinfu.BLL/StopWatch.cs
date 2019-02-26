using System;

namespace Ladasoft.Koinfu.BLL
{
    public class StopWatch
    {
        private DateTime start = DateTime.Now;

        public TimeSpan Delay(double delay)
        {
            var ts = delay - (DateTime.Now - start).TotalMilliseconds;
            return TimeSpan.FromMilliseconds(ts < 0 ? 0 : ts);
        }
    }
}
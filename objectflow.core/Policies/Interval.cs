using System.Threading;
using Rainbow.ObjectFlow.Language;

namespace Rainbow.ObjectFlow.Policies
{
#pragma warning disable 1591
    public class Interval : Policy, IInterval, ITimePart
    {
        public int MilliSecondPart { get; set; }

        public ITimePart Of
        {
            get { return this; }
        }

        public void Milliseconds(int millseconds)
        {
            MilliSecondPart = millseconds;
        }

        public void Seconds(int seconds)
        {
            Milliseconds(seconds*1000);
        }

        public void Minutes(int minutes)
        {
            Seconds(minutes*60);
        }

        internal override T Execute<T>(T current)
        {
            Thread.Sleep(MilliSecondPart);
            return current;
        }
    }
}
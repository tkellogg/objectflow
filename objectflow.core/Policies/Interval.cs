using System;
using System.Threading;
using Rainbow.ObjectFlow.Language;

namespace Rainbow.ObjectFlow.Policies
{
#pragma warning disable 1591
    public class Interval : Policy, IInterval, ITimePart, IExpression
    {
        public int MilliSecondPart { get; set; }

        internal object Parent;

        internal Interval(object parent)
        {
            Parent = parent;
        }

        public ITimePart Of
        {
            get { return this; }
        }

        public IExpression Milliseconds(int millseconds)
        {
            MilliSecondPart = millseconds;
            return this;
        }

        public IExpression Seconds(int seconds)
        {
            Milliseconds(seconds*1000);
            return this;
        }

        public IExpression Minutes(int minutes)
        {
            Seconds(minutes*60);
            return this;
        }

        internal override T Execute<T>(T current)
        {
            Thread.Sleep(MilliSecondPart);
            return current;
        }

        public IWith With
        {
            get { throw new NotImplementedException(); }
        }

        public IMerge<T> Then<T>() where T : class
        {
            return Parent as IMerge<T>;
        }
    }
}
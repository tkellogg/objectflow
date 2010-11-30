using Rainbow.ObjectFlow.Language;

#pragma warning disable 1591

namespace Rainbow.ObjectFlow.Policies
{
    public abstract class NonTerminatingPolicy : Policy, IExpression, IWith
    {
        protected Interval IntervalImp;

        public IWith With
        {
            get { return this; }
        }

        public IInterval Interval
        {
            get
            {
                IntervalImp = new Interval();
                return IntervalImp;
            }
        }
    }
}
using Rainbow.ObjectFlow.Language;

#pragma warning disable 1591

namespace Rainbow.ObjectFlow.Policies
{
    public abstract class NonTerminatingPolicy : Policy, IExpression, IWith
    {
        protected Interval IntervalImp;
        protected object Parent;
        
        public IWith With
        {
            get { return this; }
        }

        public IMerge<T> Then<T>() where T:class 
        {
            return Parent as IMerge<T>;
        }

        public IInterval Interval
        {
            get
            {
                IntervalImp = new Interval(Parent);
                return IntervalImp;
            }
        }
    }
}
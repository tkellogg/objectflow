using Rainbow.ObjectFlow.Language;

#pragma warning disable 1591
namespace Rainbow.ObjectFlow.Framework
{
    public abstract class NonTerminatingPolicy : Policy, IExpression, IWith
    {
        protected Interval _interval;

        public IWith With
        {
            get { return this; }
        }

        public IInterval Interval
        {
            get
            {
                _interval = new Interval();
                return _interval;
            }
        }
    }
}
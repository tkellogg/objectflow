using System;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Language;

#pragma warning disable 1591
namespace Rainbow.ObjectFlow.Policies
{
    public class Repeat : NonTerminatingPolicy, IRepeat
    {
        protected Repeat()
        {
            Count = 1;
        }

        internal Repeat(object parent):this()
        {
            Parent = parent;
        }

        internal override T Execute<T>(T current)
        {
            var method = Invoker as MethodInvoker<T>;
            
            if (null == method)
            {
                return current;
            }

            for (int i = 0; i < Count; i++)
            {
                current = method.Execute(current);
                if (null != IntervalImp)
                {
                    IntervalImp.Execute(current);
                }
            }

            return current;
        }

        public IExpression Times(int times)
        {
            Count = times;
            return this;
        }

        public IExpression Once()
        {
            Count = 1;
            return this;
        }

        public IExpression Twice()
        {
            Count = 2;
            return this;
        }

        public IExpression Until(Func<bool> becomesTrue)
        {
            return this;
        }

        internal int Count { get; set; }
    }
}
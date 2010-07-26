using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Language;

#pragma warning disable 1591

namespace Rainbow.ObjectFlow.Policies
{
    public class Retry : NonTerminatingPolicy, IRetryPolicy
    {
        public Retry()
        {
            Attempts(1);
        }

        internal override T Execute<T>(T current)
        {
            var operation = Invoker as OperationInvoker<T>;

            // TODO: Extend to accept FunctionInvokers
            if (null == operation)
            {
                return current;
            }

            for (int i = 0; i < Times; i++)
            {
                if (false == operation.Operation.SuccessResult)
                {
                    if (null != _interval)
                    {
                        _interval.Execute(current);
                    }
                    current = operation.Execute(current);
                }
                else
                {
                    break;
                }
            }

            return current;
        }

        public IExpression Attempts(int number)
        {
            Times = number;
            return this;
        }

        public IExpression Once()
        {
            Attempts(1);
            return this;
        }

        public IExpression Twice()
        {
            Attempts(2);
            return this;
        }

        internal int IntervalTime
        {
            get
            {
                if (null == _interval)
                    return 0;

                return _interval.MilliSeconds;
            }
        }

        internal int Times { get; set; }
    }
}
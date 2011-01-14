using System;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Language;

#pragma warning disable 1591

namespace Rainbow.ObjectFlow.Policies
{
    public class Retry : NonTerminatingPolicy, IRetryPolicy
    {
        protected Retry()
        {
            Attempts(1);
        }

        internal Retry(object parent): this()
        {
            Parent = parent;
        }
        internal override T Execute<T>(T current)
        {
            var operation = Invoker as OperationInvoker<T>;

            if (null == operation)
            {
                var function = Invoker as FunctionInvoker<T>;
                if (null != function)
                {
                    for (int i = 0; i < Count; i++)
                    {
                        try
                        {
                            if (null != IntervalImp)
                            {
                                IntervalImp.Execute(current);
                            }
                            if (!Dispatcher<T>.LastOperationSucceeded)
                                current = function.Execute(current);
                            break;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }

                    return current;
                }
            }
            else
            {
                for (int i = 0; i < Count; i++)
                {
                    if (false == operation.Operation.SuccessResult)
                    {
                        if (null != IntervalImp)
                        {
                            IntervalImp.Execute(current);
                        }
                        current = operation.Execute(current);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            
            return current;
        }

        public IExpression Attempts(int number)
        {
            Count = number;
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
                if (null == IntervalImp)
                    return 0;

                return IntervalImp.MilliSecondPart;
            }
        }

        internal int Count { get; set; }
    }
}
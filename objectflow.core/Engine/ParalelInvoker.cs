using System.Collections.Generic;
using System.Threading;

namespace Rainbow.ObjectFlow.Engine
{
    internal class ParallelInvoker<T> : MethodInvoker<T> where T : class
    {
        public IList<OperationConstraintPair<T>> RegisteredOperations = new List<OperationConstraintPair<T>>();
        private WorkflowEngine<T> _engine;

        public ParallelInvoker(WorkflowEngine<T> engine)
        {
            _engine = engine;
        }

        public ParallelInvoker()
        {
            _engine = new WorkflowEngine<T>();
        }

        public override T Execute(T data)
        {
            ManualResetEvent finishedEvent;
            ThreadProxy.ThreadCount = RegisteredOperations.Count;
            using (finishedEvent = new ManualResetEvent(false))
            {
                for (int i = 0; i < RegisteredOperations.Count; i++)
                {
                    var function = RegisteredOperations[i];

                    var threadContainer = new ThreadProxy(ref _engine, function, data, ref finishedEvent);
                    ThreadPool.QueueUserWorkItem(a => threadContainer.Start());
                }

                finishedEvent.WaitOne();
            }

            return data;
        }

        public void Add(OperationConstraintPair<T> operation)
        {
            RegisteredOperations.Add(operation);
        }

        private class ThreadProxy
        {
            public static int ThreadCount;

            private readonly OperationConstraintPair<T> _function;
            private readonly T _data;
            private readonly WorkflowEngine<T> _engine;
            private readonly ManualResetEvent _finishedEvent;

            public ThreadProxy(ref WorkflowEngine<T> engine, OperationConstraintPair<T> threadStartFunction, T parameter, ref ManualResetEvent finishedEvent)
            {
                _finishedEvent = finishedEvent;
                _function = threadStartFunction;
                _data = parameter;
                _engine = engine;
            }

            public void Start()
            {
                _engine.Execute(_function, _data);
                if (IsFinalThread())
                {
                    _finishedEvent.Set();
                }
            }

            private static bool IsFinalThread()
            {
                return 0 == Interlocked.Decrement(ref ThreadCount);
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Policies;

namespace Rainbow.ObjectFlow.Engine
{
    internal class Dispatcher<T>
    {
        internal static Stack<ICheckConstraint> ExecutionPlan { get; set; }
        internal static bool LastOperationSucceeded;

        public T Context { get; set; }
        
        static Dispatcher()
        {
            ExecutionPlan = new Stack<ICheckConstraint>();
        }

        public Dispatcher()
        {
            WfExecutionPlan._callStack = new Dictionary<int, bool>();
        }

        public virtual T Execute(IEnumerable operations)
        {
            WfExecutionPlan._callStack = new Dictionary<int, bool>();
            return Execute(operations, default(T));
        }

        public virtual T Execute(IEnumerable operations, T data)
        {
            T current = data;
            Context = data;
            foreach (OperationDuplex<T> operationPair in operations)
            {
                current = Execute(operationPair, current);
            }

            return Context;
        }

        public virtual T Execute(OperationDuplex<T> operationPair, T current)
        {
            if (ConstraintResult(operationPair.Constraint))
            {
                    try
                    {
                        current = operationPair.Command.Execute(current);
                        LastOperationSucceeded = true;
                    }
                    catch (Exception)
                    {
                        LastOperationSucceeded = false;
                    }

                current = ExecutePolicies(operationPair.Command, current);
            }

            Context = current;

            return Context;
        }

        public virtual T Execute(OperationDuplex<T> operationPair)
        {
            return Execute(operationPair, default(T));
        }

        private T ExecutePolicies(MethodInvoker<T> method, T current)
        {
            if (method.Policies.Count > 0)
            {
                foreach (var policy in method.Policies)
                {
                    var abstractPolicy = policy as Policy;

                    Debug.Assert(abstractPolicy != null, "Policy should always inherit from abstract class");
                    abstractPolicy.SetInvoker(method);
                        current = abstractPolicy.Execute(current);                        
                }
            }

            return current;
        }

        private static bool ConstraintResult(ICheckConstraint constraint)
        {
            return null == constraint || constraint.Matches();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Rainbow.ObjectFlow.Constraints;
using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Policies;

namespace Rainbow.ObjectFlow.Engine
{
    internal class WorkflowEngine<T>
    {
        internal static Stack<ICheckConstraint> CallStack { get; set; }

        static WorkflowEngine()
        {
            CallStack = new Stack<ICheckConstraint>();
        }

        public virtual T Execute(IEnumerable operations)
        {
            return Execute(operations, default(T));
        }

        public virtual T Execute(IEnumerable operations, T data)
        {
            T current = data;

            foreach (OperationConstraintPair<T> operationPair in operations)
            {
                current = Execute(operationPair, current);
            }

            return current;
        }

        public virtual T Execute(OperationConstraintPair<T> operationPair)
        {
            return Execute(operationPair, default(T));
        }

        public virtual T Execute(OperationConstraintPair<T> operationPair, T current)
        {
            if (ConstraintResult(operationPair.Constraint as Condition))
            {
                current = operationPair.Command.Execute(current);

                current = ExecutePolicies(operationPair.Command, current);
            }

            return current;
        }

        private T ExecutePolicies(MethodInvoker<T> method, T current)
        {
            if (method.Policies.Count > 0)
            {
                foreach (var policy in method.Policies)
                {
                    var abstractPolicy = policy as Policy;

                    Debug.Assert(abstractPolicy != null, "Policy should always inherit from abstract class");
                    abstractPolicy.SetParent(method);
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
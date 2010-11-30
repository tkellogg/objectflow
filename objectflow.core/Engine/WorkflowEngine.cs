using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Rainbow.ObjectFlow.Constraints;
using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Policies;

[assembly: InternalsVisibleTo("objectflow.core.tests, PublicKey=002400000480000094000000060200000024000052534131000400000100010063c5cddc6ccfdb91c0753f9448f818b61c97a3e12a94ff39bce6cbe4f574716e79c5b847ece96ff4147c2566b8e9555e82fe09ebe6eef2e7ef5dc1cade029c64c4880ae76bd2ecf4d99caf152c2b438e87d2561d04ae039baa4335b9980277111cdd73cd207eeae1906fcb96f5cfc28114acad468b8ca75a96935415665266b6")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7")]

namespace Rainbow.ObjectFlow.Engine
{
    internal class WorkflowEngine<T>
    {
        internal static Stack<ICheckConstraint> ExecutionPlan { get; set; }
        public T Context { get; set; }

        static WorkflowEngine()
        {
            ExecutionPlan = new Stack<ICheckConstraint>();                      
        }

        public WorkflowEngine()
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

            foreach (OperationDuplex<T> operationPair in operations)
            {
                if (operationPair.Command.IsContextBound)
                {
                    var op = operationPair.Command as FunctionInvoker<T>;
                    if (op != null)
                    {
                        if (ConstraintResult(operationPair.Constraint as Condition))
                        {
                            op.Execute();
                            current = Context;
                            current = ExecutePolicies(operationPair.Command, current);
                        }
                    }
                }
                else
                {
                    current = Execute(operationPair, current);
                    Context = current;
                }
            }

            return current;
        }

        public virtual T Execute(OperationDuplex<T> operationPair)
        {
            return Execute(operationPair, default(T));
        }

        public virtual T Execute(OperationDuplex<T> operationPair, T current)
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
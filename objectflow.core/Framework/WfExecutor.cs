using System.Collections;
using System.Collections.Generic;
using Rainbow.ObjectFlow.Constraints;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Framework
{
    /// <summary>
    /// Workflow execution engine
    /// </summary>
    /// <typeparam name="T">generic type the engine will work with</typeparam>
    internal class WfExecutor<T>
    {
        private static IEnumerable<T> Execute(IOperation<T> operation, CheckConstraint constraint, IEnumerable<T> input)
        {
            if (constraint == null || constraint.Matches())
            {
                input = operation.Execute(input);
                var op = operation as BasicOperation<T>;

                if (op != null)
                {
                    op.SetSuccessResult(true);
                }
            }

            return input;
        }

        /// <summary>
        /// Execute the operations
        /// </summary>
        /// <param name="operations">operations to execute</param>
        /// <returns>Workflow results</returns>
        public static IEnumerable<T> Execute(ICollection operations)
        {
            IEnumerable<T> current = new List<T>();

            foreach (OperationConstraintPair<T> operationPair in operations)
            {
                var operation = operationPair.Operation;
                var constraint = operationPair.Constraint;
                current = Execute(operation, constraint, current);

            }

            if (current == null)
            {
                return current;
            }

            var enumerator = current.GetEnumerator();
            while (enumerator.MoveNext())
            {
            }

            return current;
        }
    }
}

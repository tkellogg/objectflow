using System.Collections;
using Rainbow.ObjectFlow.Constraints;

namespace Rainbow.ObjectFlow.Framework
{
    /// <summary>
    /// Workflow execution engine
    /// </summary>
    /// <typeparam name="T">generic type the engine will work with</typeparam>
    internal class WfExecutor<T>
    {
        /// <summary>
        /// Execute the operations
        /// </summary>
        /// <param name="operations">operations to execute</param>
        /// <returns>Workflow results</returns>
        public static T Execute(ICollection operations)
        {
            return Execute(operations, default(T));
        }

        public static T Execute(ICollection operations, T data)
        {
            T current = data;

            foreach (OperationConstraintPair<T> operationPair in operations)
            {
                if (ConstaintResult(operationPair.Constraint))
                {
                    current = operationPair.Command.Execute(current);
                }
            }

            return current;
        }

        private static bool ConstaintResult(CheckConstraint constraint)
        {
            return constraint == null || constraint.Matches();
        }
    }
}

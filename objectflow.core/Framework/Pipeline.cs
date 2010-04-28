using System.Collections.Generic;
using Rainbow.ObjectFlow.Constraints;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Framework
{
    /// <summary>
    /// Pipelines are composed of the generic IOperation.  A pipeline
    /// controls the workflow
    /// </summary>
    /// <typeparam name="T">The type this pipeline contains</typeparam>
    public abstract class Pipeline<T> : IWorkFlow<T> where T : class
    {
        /// <summary>
        /// Operations that have been added to the execution plan
        /// </summary>
        internal Queue<OperationConstraintPair<T>> RegisteredOperations;

        /// <summary>
        /// default constructor
        /// </summary>
        protected Pipeline()
        {
            RegisteredOperations = new Queue<OperationConstraintPair<T>>();
        }

        /// <summary>
        /// Adds an operation into the execution plan
        /// </summary>
        /// <param name="operation">The operation to execute as a generic of IOperation</param>
        /// <returns>this object to enable chaining</returns>
        public Pipeline<T> Execute(IOperation<T> operation)
        {
            Check.IsNotNull(operation, "operation");

            AddOperation(operation, null);

            return this;
        }

        /// <summary>
        /// Adds an operation into the execution plan given the constraint.  The constraint runs if
        /// the constraint evaluates to true.
        /// </summary>
        /// <param name="operation">The operation to execute as a generic of IOperation</param>
        /// <param name="constraint">The constraint to evaluate</param>
        /// <returns>this object to enable chaining</returns>
        public Pipeline<T> Execute(IOperation<T> operation, ICheckContraint constraint)
        {
            Check.IsNotNull(operation, "operation");
            Check.IsNotNull<T>(constraint, "constraint");

            AddOperation(operation, constraint);

            return this;
        }

        private void AddOperation(IOperation<T> operation, ICheckContraint constraint)
        {
            var operationPair = new OperationConstraintPair<T>(operation as BasicOperation<T>, constraint);
            RegisteredOperations.Enqueue(operationPair);
        }

        /// <summary>
        /// Return an element from the results returned by the Start method.
        /// </summary>
        /// <param name="results">Results returned from the Start method.</param>
        /// <param name="index">index of the required data item.</param>
        /// <returns>Specified data item if it exists, uninstantiated object of type T otherwise.</returns>
        public static T GetItem(IEnumerable<T> results, int index)
        {
            var list = results as IList<T>;

            if (list == null || (list.Count < index))
            {
                return default(T);
            }

            return list[index];
        }

        /// <summary>
        /// Begins the execution of a workflow
        /// </summary>
        /// <returns>The data after being transformed by the Operation objects</returns>
        /// <remarks>
        /// The concrete implementation is responsible for definng how the data is passed to the pipeline.
        /// A common implementation is to use a constructor and an operation that returns the data with no transformations
        /// </remarks>
        /// <example>
        /// show loadlist implementation.
        /// </example>
        public IEnumerable<T> Start()
        {
            IEnumerable<T> current = new List<T>();

            while (RegisteredOperations.Count > 0)
            {
                var operationPair = RegisteredOperations.Dequeue();
                var operation = operationPair.Operation;
                var constraint = operationPair.Constraint;
                current = Do(operation, constraint as CheckConstraint, current);
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

        private static IEnumerable<T> Do(IOperation<T> operation, CheckConstraint constraint, IEnumerable<T> input)
        {
            if (constraint == null || constraint.Matches())
            {
                input = operation.Execute(input);
                var op = operation as BasicOperation<T>;
                // TODO set success on an event from operation.
                if (op != null)
                {
                    op.SetSuccessResult(true);
                }
            }

            return input;
        }
    }
}
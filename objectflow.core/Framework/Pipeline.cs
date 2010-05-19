using System.Collections.Generic;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Framework
{
    /// <summary>
    /// Pipelines are composed of generic IOperations.  A pipeline
    /// controls the workflow whereas the IOperation encapsalates logic.
    /// </summary>
    /// <typeparam name="T">The type this pipeline contains</typeparam>
    public class Pipeline<T> : IWorkFlow<T> where T : class
    {
        /// <summary>
        /// Operations that have been added to the execution plan
        /// </summary>
        internal Queue<OperationConstraintPair<T>> RegisteredOperations;

        /// <summary>
        /// default constructor
        /// </summary>
        public Pipeline()
        {
            RegisteredOperations = new Queue<OperationConstraintPair<T>>();
        }

        /// <summary>
        /// Adds an operation into the execution plan
        /// </summary>
        /// <param name="operation">The operation to execute as a generic of IOperation</param>
        /// <returns>this object to enable chaining</returns>
        /// <remarks>Operations must inherit from the BasicOperation class</remarks>
        public virtual Pipeline<T> Execute(IOperation<T> operation)
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
        /// <remarks>Operations must inherit from the BasicOperation class</remarks>
        public virtual Pipeline<T> Execute(IOperation<T> operation, ICheckContraint constraint)
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

            if (null == list || 0 == list.Count || (list.Count < index))
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
        public virtual IEnumerable<T> Start()
        {
            return WfExecutor<T>.Execute(RegisteredOperations);
        }
    }
}
using System;
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

            var operationPair = new OperationConstraintPair<T>(new OCommand<T>(operation as BasicOperation<T>));
            RegisteredOperations.Enqueue(operationPair);

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

            var operationPair = new OperationConstraintPair<T>(new OCommand<T>(operation as BasicOperation<T>), constraint);
            RegisteredOperations.Enqueue(operationPair);

            return this;
        }

        /// <summary>
        /// Adds a function into the execution plan
        /// </summary>
        /// <param name="function">Function to add</param>
        /// <returns>this</returns>
        public Pipeline<T> Execute(Func<T, T> function)
        {
            Check.IsNotNull(function, "function");

            var operationPair = new OperationConstraintPair<T>(new FCommand<T>(function));
            RegisteredOperations.Enqueue(operationPair);

            return this;
        }

        /// <summary>
        /// Adds a function into the execution plan
        /// </summary>
        /// <param name="function">Function to add</param>
        /// <param name="constraint">constraint defines if function will be executed</param>
        /// <returns>this</returns>
        public Pipeline<T> Execute(Func<T, T> function, ICheckContraint constraint)
        {
            Check.IsNotNull(function, "function");
            Check.IsNotNull<T>(constraint, "constraint");

            var operationPair = new OperationConstraintPair<T>(new FCommand<T>(function), constraint);
            RegisteredOperations.Enqueue(operationPair);

            return this;
        }

        /// <summary>
        /// Begins the execution of a workflow
        /// </summary>
        /// <returns>The data after being transformed by the Operation objects</returns>
        /// <remarks>
        /// The concrete implementation is responsible for definng how the data is passed to the pipeline.
        /// A common implementation is to use a constructor and an operation that returns the data with no transformations
        /// </remarks>
        public virtual T Start()
        {
            return WfExecutor<T>.Execute(RegisteredOperations);
        }

        /// <summary>
        /// Runs the workflow definition
        /// </summary>
        /// <param name="data">data to transform</param>
        /// <returns>Result of the workflow</returns>
        public virtual T Start(T data)
        {
            return WfExecutor<T>.Execute(RegisteredOperations, data);
        }

    }
}
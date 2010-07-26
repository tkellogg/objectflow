using System;
using Rainbow.ObjectFlow.Language;

namespace Rainbow.ObjectFlow.Interfaces
{
    /// <summary>
    /// Interface for the workflow pipeline
    ///</summary>
    public interface IWorkflow<T> : IHideObjectMembers where T : class
    {
        ///<summary>
        /// Chained operations will be executed concurrently
        ///</summary>
        ICompose<T> And { get; }

        ///<summary>
        /// Merges concurrent operations.  By default engine waits for all parallel operations to finish before executing subsequent operations sequentially
        ///</summary>
        /// <remarks>The default is to wait for all concurrent operatins to finish before continuing with sequential the following sequential operations</remarks>
        IMerge<T> Then();

        /// <summary>
        /// Adds operations into the workflow definition
        /// </summary>
        /// <param name="operation">The operation to add</param>
        IWorkflow<T> Do(IOperation<T> operation);

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The function to add</param>
        IWorkflow<T> Do(Func<T, T> function);

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The funciton to add</param>
        /// <param name="constraint"></param>
        IWorkflow<T> Do(Func<T, T> function, ICheckConstraint constraint);

        /// <summary>
        /// Adds an operation into the execution path 
        /// </summary>
        /// <param name="operation">operatio to add</param>
        /// <param name="constraint">constraint that determines if the operation is executed</param>
        IWorkflow<T> Do(IOperation<T> operation, ICheckConstraint constraint);

        /// <summary>
        /// Adds a sub-workflow into the execution path
        /// </summary>
        /// <param name="workflow">The function to add</param>
        IWorkflow<T> Do(IWorkflow<T> workflow);

        /// <summary>
        /// Adds a sub-workflow into the execution path
        /// </summary>
        /// <param name="workflow">The funciton to add</param>
        /// <param name="constraint">The condition whose evaluation determines if the workflow is executed</param>
        IWorkflow<T> Do(IWorkflow<T> workflow, ICheckConstraint constraint);

        /// <summary>
        /// Runs the workflow definition
        /// </summary>
        /// <returns>Result of the workflow</returns>
        T Start();

        /// <summary>
        /// Runs the workflow definition
        /// </summary>
        /// <param name="data">data to transform</param>
        /// <returns>Result of the workflow</returns>
        T Start(T data);

        ///<summary>
        /// Attempt the operation again if it does not finish successfully
        ///</summary>
        /// <remarks>Currently ignores any operations that do not inherit from BasicOperation</remarks>
        IRetryPolicy Retry();
    }
}
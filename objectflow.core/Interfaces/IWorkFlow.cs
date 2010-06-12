using System;
using Rainbow.ObjectFlow.Framework;

namespace Rainbow.ObjectFlow.Interfaces
{
    /// <summary>
    /// Interface for the workflow pipeline
    /// </summary>
    /// <typeparam name="T">Type of data the pipeline contains</typeparam>
    public interface IWorkflow<T> where T : class
    {
        /// <summary>
        /// Adds operations into the workflow definition
        /// </summary>
        /// <param name="operation">The operation to add</param>
        /// <returns>this</returns>
        Workflow<T> Do(IOperation<T> operation);

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The function to add</param>
        /// <returns>this</returns>
        Workflow<T> Do(Func<T, T> function);

        /// <summary>
        /// Adds a function into the execution path
        /// </summary>
        /// <param name="function">The funciton to add</param>
        /// <param name="constraint"></param>
        /// <returns>this</returns>
        Workflow<T> Do(Func<T, T> function, ICheckContraint constraint);

        /// <summary>
        /// Adds an operation into the execution path 
        /// </summary>
        /// <param name="operation">operatio to add</param>
        /// <param name="constraint">constraint that defines if the operation is executed</param>
        /// <returns>this</returns>
        Workflow<T> Do(IOperation<T> operation, ICheckContraint constraint);

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
    }
}
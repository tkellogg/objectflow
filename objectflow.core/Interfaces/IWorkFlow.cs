using System.Collections.Generic;
using Rainbow.ObjectFlow.Framework;

namespace Rainbow.ObjectFlow.Interfaces
{
    /// <summary>
    /// Interface for the workflow pipeline
    /// </summary>
    /// <typeparam name="T">Type of data the pipeline contains</typeparam>
    public interface IWorkFlow<T> where T : class
    {
        /// <summary>
        /// Adds operations into the execution plan
        /// </summary>
        /// <param name="operation">The operation to add</param>
        /// <returns>Returns itself</returns>
        Pipeline<T> Execute(IOperation<T> operation);

        /// <summary>
        /// Runs the workflow definition
        /// </summary>
        /// <returns>Array of data the workflow has been transforming</returns>
        IEnumerable<T> Start();
    }
}
using System.Collections.Generic;

namespace Rainbow.ObjectFlow.Interfaces
{
    /// <summary>
    /// Defines the interface of items in a pipeline.
    /// </summary>
    /// <typeparam name="T">Type of data the pipeline contains</typeparam>
    public interface IOperation<T>
    {
        /// <summary>
        /// Executes the operation
        /// </summary>
        /// <param name="data">The data the operation will transform</param>
        /// <returns>input data transformed by the operation</returns>
        T Execute(T data);
    }
}
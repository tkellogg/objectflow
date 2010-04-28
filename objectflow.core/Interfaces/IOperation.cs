using System.Collections.Generic;

namespace Rainbow.ObjectFlow.Interfaces
{
    /// <summary>
    /// Defines the interface of items in a pipeline.
    /// </summary>
    public interface IOperation<T>
    {
        IEnumerable<T> Execute(IEnumerable<T> operations);
    }
}
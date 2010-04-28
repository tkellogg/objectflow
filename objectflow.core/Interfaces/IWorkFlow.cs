using System.Collections.Generic;
using Rainbow.ObjectFlow.Framework;

namespace Rainbow.ObjectFlow.Interfaces
{
    public interface IWorkFlow<T> where T : class
    {
        Pipeline<T> Execute(IOperation<T> operation1);

        IEnumerable<T> Start();
    }
}
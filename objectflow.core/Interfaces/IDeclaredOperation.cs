using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Rainbow.ObjectFlow.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDeclaredOperation
    {
        /// <summary>
        /// Executing the operations returned from this method is equivalent to branching
        /// down this path
        /// </summary>
        /// <returns></returns>
        IEnumerable GetRemainingOperations();
    }
}

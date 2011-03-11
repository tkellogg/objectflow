using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Stateful
{
    /// <summary>
    /// Factory for processing objects through a workflow
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IWorkflowFactory<T>
    {
        /// <summary>
        /// Proxesses the object through the correct portion of the workflow and 
        /// returns the result.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        T Process(T initializer);
    }
}

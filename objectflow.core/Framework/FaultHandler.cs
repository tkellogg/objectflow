using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Framework
{
    /// <summary>
    /// Default fault handler implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FaultHandler<T> : Rainbow.ObjectFlow.Interfaces.IFaultHandler<T>
    {

        #region IFaultHandler<T> Members

        /// <summary>
        /// Called when an exception occurs in a workflow step. Different return values 
        /// determine what should happen in the control flow
        /// </summary>
        /// <param name="ex">The exception that caused the fault</param>
        /// <param name="data">The data object being manipulated by the workflow</param>
        /// <returns>The FaultLevel indicating how workflow execution should continue</returns>
        public virtual Interfaces.FaultLevel HandleFault(Exception ex, T data)
        {
            return Interfaces.FaultLevel.Handled;
        }

        #endregion
    }
}

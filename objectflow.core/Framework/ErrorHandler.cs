using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Framework
{
    /// <summary>
    /// Default error handler implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ErrorHandler<T> : Rainbow.ObjectFlow.Interfaces.IErrorHandler<T>
    {

        #region IErrorHandler<T> Members

        /// <summary>
        /// Called when an exception occurs in a workflow step. Different return values 
        /// determine what should happen in the control flow
        /// </summary>
        /// <param name="ex">The exception that caused the error</param>
        /// <param name="data">The data object being manipulated by the workflow</param>
        /// <returns>The ErrorLevel indicating how workflow execution should continue</returns>
        public virtual Interfaces.ErrorLevel Handle(Exception ex, T data)
        {
            return Interfaces.ErrorLevel.Handled;
        }

        #endregion
    }
}

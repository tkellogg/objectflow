using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Interfaces
{
    /// <summary>
    /// Describes the level of which a fault was handled
    /// </summary>
    public enum ErrorLevel
    {
        /// <summary>The operation should be considered successful</summary>
        Ignored,
        /// <summary>The operation should be considered not successful but execution
        /// of the workflow can still continue</summary>
        Handled,
        /// <summary>Fault was fatal, workflow execution will immediately abort. This
        /// fault level should not be taken lightly. Fatalities should be avoided.</summary>
        Fatal
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IErrorHandler<T>
    {
		/// <summary>
		/// Indicates that exceptions will never be caught. Default is false
		/// </summary>
		bool Strict { get; set; }

        /// <summary>
        /// Called when an exception occurs in a workflow step. Different return values 
        /// determine what should happen in the control flow
        /// </summary>
        /// <param name="ex">The exception that caused the fault</param>
        /// <param name="data">The data object being manipulated by the workflow</param>
        /// <returns>The FaultLevel indicating how workflow execution should continue</returns>
        ErrorLevel Handle(Exception ex, T data);
    }
}

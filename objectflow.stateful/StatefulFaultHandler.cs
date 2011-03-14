using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Framework;

namespace Rainbow.ObjectFlow.Stateful
{
    /// <summary>
    /// Fault handler with modifications for stateful workflows. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StatefulFaultHandler<T> : FaultHandler<T>
    {
        /// <summary>
        /// Handles UnallowedTransitionException fatally, making it possible to 
        /// easily break out of a workflow
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public override Interfaces.FaultLevel HandleFault(Exception ex, T data)
        {
            if (ex is UnallowedTransitionException)
                return Interfaces.FaultLevel.Fatal;
            return base.HandleFault(ex, data);
        }
    }
}

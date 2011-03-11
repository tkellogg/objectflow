using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Stateful
{
    /// <summary>
    /// A transition was disallowed by an ITransitionGateway.
    /// </summary>
    public class UnallowedTransitionException : Exception
    {
        internal UnallowedTransitionException(string msg, params object[] parameters)
            :base (string.Format(msg, parameters))
        {
        }
    }
}

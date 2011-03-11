using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Stateful
{
    /// <summary>
    /// Security provider object that allows or disallows a transition from 
    /// occurring.
    /// </summary>
    public interface ITransitionGateway
    {
        /// <summary>
        /// Indicates that a transition is allowed to occur
        /// </summary>
        /// <param name="transition"></param>
        /// <returns><c>true</c> if the transition is allowed</returns>
        bool IsTransitionAllowed(ITransition transition);
    }
}

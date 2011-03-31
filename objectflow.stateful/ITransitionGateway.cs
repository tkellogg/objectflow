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
        /// Takes a list of transitions (already limited to only transitions where To,
        /// From, and WorkflowId are the same) and filters out transitions that aren't
        /// allowed for the current user.
        /// </summary>
        /// <param name="transitions">A set of transitions that could happen</param>
        /// <returns>A subset of <c>transitions</c> that is actually allowed. Remove
        /// any transitions from this list that are not allowed.</returns>
        IEnumerable<ITransition> AllowTransitions(IList<ITransition> transitions);
    }
}

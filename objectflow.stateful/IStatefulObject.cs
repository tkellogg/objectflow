using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Stateful
{
    /// <summary>
    /// <para>
    /// A persistable object that represents where a workflow was left off. By passing
    /// one of these objects to a IStatefullWorkflow we can resume the workflow where
    /// it was last left off. 
    /// </para>
    /// <para>
    /// The state ID is left as an object but should contain a meaningful implementation 
    /// of <c>.Equals(object)</c>. Therefore, string, int, and Guid are all great choices
    /// for state and workflow identifiers. If you choose to make a custom object for this
    /// purpose, be sure to provide a meaningful <c>Equals(object)</c> operation.
    /// </para>
    /// </summary>
    public interface IStatefulObject
    {
        /// <summary>
        /// Gets the identifer that represents the object's state. A workflow uses this
        /// identifier to restore the state of the object in the 
        /// </summary>
        /// <param name="workflowId">The identifier for the workflow. If this value is
        /// null, this object is expected to either be valid for only a single workflow
        /// or should throw a NotSupportedException</param>
        /// <returns>The state within a workflow that this object is currently in. If the 
        /// object isn't in a worflow identified by <c>workflowId</c>, this method will 
        /// return null</returns>
        object GetStateId(object workflowId);

        /// <summary>
        /// Gets the identifer that represents the object's state. A workflow uses this
        /// identifier to restore the state of the object in the 
        /// </summary>
        /// <param name="workflowId">The identifier for the workflow. If this value is
        /// null, this object is expected to either be valid for only a single workflow
        /// or should throw a NotSupportedException</param>
        /// <param name="stateId">The state identifier for the workflow. </param>
        void SetStateId(object workflowId, object stateId);
    }
}

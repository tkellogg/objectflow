using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Interfaces
{
    /// <summary>
    /// A persistable object that represents where a workflow was left off. By passing
    /// one of these objects to a IStatefullWorkflow we can resume the workflow where
    /// it was last left off.
    /// </summary>
    public interface IStatefulObject
    {
        /// <summary>
        /// The identifer that represents the object's state. A workflow uses this
        /// identifier to restore the state of the object in the 
        /// </summary>
        object StateId { get; set; }

        /// <summary>
        /// Persist the state of the workflow
        /// </summary>
        void SaveState();
    }
}

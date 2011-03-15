using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Stateful
{
    /// <summary>
    /// Represents a transition from one Yield point to the next. With
    /// the possibility of branching there may be several transition paths to get
    /// from point A to B.
    /// </summary>
    public interface ITransition
    {
        /// <summary>
        /// The workflow for which this applies
        /// </summary>
        object WorkflowId { get; }

        /// <summary>
        /// Gets the starting transition point
        /// </summary>
        object From { get; }

        /// <summary>
        /// Gets the ending transition point
        /// </summary>
        object To { get; }
    }
}

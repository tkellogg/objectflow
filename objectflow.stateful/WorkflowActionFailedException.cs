using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Stateful
{
	/// <summary>
	/// Thrown when a workflow step fails outright due to a failed assertion
	/// </summary>
	public class WorkflowActionFailedException : ApplicationException
	{
		/// <summary>
		/// Create a new WorkflowActionFailedException
		/// </summary>
		/// <param name="message"></param>
		public WorkflowActionFailedException(string message)
			: base(message)
		{ }
	}
}

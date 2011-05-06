using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Stateful.tests.TransitionRulesWithWorkflow
{
	public class TestObject : IStatefulObject
	{

		#region IStatefulObject Members

		public object GetStateId(object workflowId)
		{
			return State;
		}

		public void SetStateId(object workflowId, object stateId)
		{
			State = stateId;
		}

		#endregion

		public object State { get; set; }
	}

	class TestFixtures
	{
		/// <summary>
		/// Workflow with 3 segments
		/// </summary>
		public static IStatefulWorkflow<TestObject> SetupWorkflow(IStatefulWorkflow<TestObject> workflow)
		{
				return workflow
					.Yield("First")
					.Yield("Last");
		}
	}
}

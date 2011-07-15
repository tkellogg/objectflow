using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Rainbow.ObjectFlow.Stateful.tests.PossibleTransitions
{
	[TestFixture]
	class WhenGivenBreakWithStatus
	{
		#region Types used for testing
		class Entity : IStatefulObject
		{
			public object state;
			public object GetStateId(object workflowId)
			{
				return state;
			}

			public void SetStateId(object workflowId, object stateId)
			{
				state = stateId;
			}
		}
		#endregion

		[Observation]
		public void it_recognizes_that_branch_within_PossibleTransitions_collection()
		{
			var wf = new StatefulWorkflow<Entity>()
				.Yield("broke")
				.Yield("begin")
				.When(x => true).BreakWithStatus("broke")
				.Yield("didn't break");

			Assert.That(wf.PossibleTransitions.Any(x => (string)x.From == "begin" && (string)x.To == "broke"),
				"PossibleTransitions should contain the potential transition from 'begin' to 'broke'");
		}
	}
}

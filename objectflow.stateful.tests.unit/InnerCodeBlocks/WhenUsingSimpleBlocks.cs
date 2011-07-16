using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Rainbow.ObjectFlow.Stateful.tests.InnerCodeBlocks
{
	[TestFixture]
	class WhenUsingSimpleBlocks
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
		[TestCase(false, Result = 0)]
		[TestCase(true, Result = 2)]
		public int multiple_workflow_steps_are_executed_conditionally(bool branch)
		{
			int hitInnerBranch = 0, hitOutterBranch = 0;
			var workflow = new StatefulWorkflow<Entity>()
				.When(x => branch).Do(wf =>
				{
					wf.Do(x => hitInnerBranch++);
					wf.Do(x => hitInnerBranch++);
				})
				.Do(x => hitOutterBranch++)
				.Yield("end");

			var obj = new Entity();
			workflow.Start(obj);
			hitOutterBranch.ShouldBe(1);
			return hitInnerBranch;
		}
	}
}

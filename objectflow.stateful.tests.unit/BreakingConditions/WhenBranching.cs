using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Helpers;
using NUnit.Framework;

namespace Rainbow.ObjectFlow.Stateful.tests.BreakingConditions
{
	class WhenBranching : Specification
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

		#region branches using simple conditions
		[Observation]
		public void it_can_branch_using_positive_When()
		{
			var point = Declare.Step();

			var wf = new StatefulWorkflow<Entity>()
				.Define(point)
				.Yield("branched")
				.Yield("start")
				.When(x => true).BranchTo(point)
				.Yield("didn't branch");

			var obj = new Entity() { state = "start" };
			wf.Start(obj);
			Assert.That(obj.GetStateId(null), Is.EqualTo("branched"));
		}

		[Observation]
		public void it_can_branch_using_negative_When()
		{
			var point = Declare.Step();

			var wf = new StatefulWorkflow<Entity>()
				.Define(point)
				.Yield("branched")
				.Yield("start")
				.When(x => false).BranchTo(point)
				.Yield("didn't branch");

			var obj = new Entity() { state = "start" };
			wf.Start(obj);
			Assert.That(obj.GetStateId(null), Is.EqualTo("didn't branch"));
		}

		[Observation]
		public void it_can_branch_using_negative_Unless()
		{
			var point = Declare.Step();

			var wf = new StatefulWorkflow<Entity>()
				.Define(point)
				.Yield("branched")
				.Yield("start")
				.Unless(x => false).BranchTo(point)
				.Yield("didn't branch");

			var obj = new Entity() { state = "start" };
			wf.Start(obj);
			Assert.That(obj.GetStateId(null), Is.EqualTo("branched"));
		}

		[Observation]
		public void it_can_branch_using_positive_Unless()
		{
			var point = Declare.Step();

			var wf = new StatefulWorkflow<Entity>()
				.Define(point)
				.Yield("branched")
				.Yield("start")
				.Unless(x => true).BranchTo(point)
				.Yield("didn't branch");

			var obj = new Entity() { state = "start" };
			wf.Start(obj);
			Assert.That(obj.GetStateId(null), Is.EqualTo("didn't branch"));
		}
		#endregion

		#region branches using more complex, parameterized conditions
		[Observation]
		public void it_can_branch_using_positive_When_with_parameters()
		{
			var point = Declare.Step();

			var wf = new StatefulWorkflow<Entity>()
				.Define(point)
				.Yield("branched")
				.Yield("start")
				.When((x, opts) => (bool)opts["shouldBranch"]).BranchTo(point)
				.Yield("didn't branch");

			var obj = new Entity() { state = "start" };
			wf.StartWithParams(obj, new { shouldBranch = true });
			Assert.That(obj.GetStateId(null), Is.EqualTo("branched"));
		}

		[Observation]
		public void it_can_branch_using_negative_When_with_parameters()
		{
			var point = Declare.Step();

			var wf = new StatefulWorkflow<Entity>()
				.Define(point)
				.Yield("branched")
				.Yield("start")
				.When((x, opts) => (bool)opts["shouldBranch"]).BranchTo(point)
				.Yield("didn't branch");

			var obj = new Entity() { state = "start" };
			wf.StartWithParams(obj, new { shouldBranch = false });
			Assert.That(obj.GetStateId(null), Is.EqualTo("didn't branch"));
		}

		[Observation]
		public void it_can_branch_using_negative_Unless_with_parameters()
		{
			var point = Declare.Step();

			var wf = new StatefulWorkflow<Entity>()
				.Define(point)
				.Yield("branched")
				.Yield("start")
				.Unless((x, opts) => (bool)opts["shouldBranch"]).BranchTo(point)
				.Yield("didn't branch");

			var obj = new Entity() { state = "start" };
			wf.StartWithParams(obj, new { shouldBranch = false });
			Assert.That(obj.GetStateId(null), Is.EqualTo("branched"));
		}

		[Observation]
		public void it_can_branch_using_positive_Unless_with_parameters()
		{
			var point = Declare.Step();

			var wf = new StatefulWorkflow<Entity>()
				.Define(point)
				.Yield("branched")
				.Yield("start")
				.Unless((x, opts) => (bool)opts["shouldBranch"]).BranchTo(point)
				.Yield("didn't branch");

			var obj = new Entity() { state = "start" };
			wf.StartWithParams(obj, new { shouldBranch = true });
			Assert.That(obj.GetStateId(null), Is.EqualTo("didn't branch"));
		}
		#endregion
	}
}

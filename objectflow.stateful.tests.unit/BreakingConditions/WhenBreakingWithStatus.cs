using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;

namespace Rainbow.ObjectFlow.Stateful.tests.BreakingConditions
{
	[TestFixture]
	class WhenBreakingWithStatus
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
		public void it_can_break_with_status()
		{
			bool branched = false;
			var wf = new StatefulWorkflow<Entity>()
				.Do(x => branched = true)
				.Yield("broke")
				.Yield("begin")
				.When(x => true).BreakWithStatus("broke")
				.Yield("didn't break");

			var obj = new Entity() { state = "begin" };
			wf.Start(obj);
			obj.state.ShouldBe("broke");
			Assert.That(!branched);
		}

		/// <summary>
		/// This test, just because this is BDD so we're specifying behavior. This would obviously cause
		/// problems when you start back up
		/// </summary>
		[Observation]
		public void it_will_let_you_break_with_a_nonexistent_state()
		{
			bool branched = false;
			var wf = new StatefulWorkflow<Entity>()
				.Do(x => branched = true)
				.Yield("broke")
				.Yield("begin")
				.When(x => true).BreakWithStatus("nonexistent")
				.Yield("didn't break");

			var obj = new Entity() { state = "begin" };
			wf.Start(obj);
			obj.state.ShouldBe("nonexistent");
			Assert.That(!branched);
		}

		[Observation]
		public void it_observes_security_rules()
		{
			var security = new Mock<ITransitionGateway>();
			security.Setup(x => x.AllowTransitions(It.IsAny<IList<ITransition>>()))
				.Returns<IList<ITransition>>(x => x.Where(y => (string)y.From == "begin" && (string)y.To == "didn't break"));

			bool branched = false;
			var wf = new StatefulWorkflow<Entity>(null, security.Object)
				.Do(x => branched = true)
				.Yield("broke")
				.Yield("begin")
				.When(x => true).BreakWithStatus("broke")
				.Yield("didn't break");

			var obj = new Entity() { state = "begin" };
			Assert.Throws<UnallowedTransitionException>(() => wf.Start(obj));
			obj.state.ShouldBe("begin");
			Assert.That(!branched);
		}
	}
}

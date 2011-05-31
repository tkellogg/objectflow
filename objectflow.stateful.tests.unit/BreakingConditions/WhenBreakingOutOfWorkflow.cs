using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Rainbow.ObjectFlow.Stateful.tests.BreakingConditions
{
	[TestFixture]
	class WhenBreakingOutOfWorkflow
	{
		#region types used for testing
		class TestObject : IStatefulObject
		{

			#region IStatefulObject Members

			public string ID { get; set; }

			public object GetStateId(object workflowId)
			{
				return ID;
			}

			public void SetStateId(object workflowId, object stateId)
			{
				ID = (string)stateId;
			}

			#endregion
		}
		#endregion

		[Test]
		public void it_recognizes_positive_predicate()
		{
			var t = new TestObject();

			new StatefulWorkflow<TestObject>()
				.When(x => false)
				.Yield("didn't stop")
				.Start(t);

			t.ID.ShouldBeNull();
		}

		[Test]
		public void it_recognizes_negative_predicate()
		{
			var t = new TestObject();

			new StatefulWorkflow<TestObject>()
				.When(x => true)
				.Yield("didn't stop")
				.Start(t);

			t.ID.ShouldBe("didn't stop");
		}

		[Test]
		public void Unless_works_oposite_when_passing()
		{
			var t = new TestObject();

			new StatefulWorkflow<TestObject>()
				.Unless(x => true)
				.Yield("didn't stop")
				.Start(t);

			t.ID.ShouldBeNull();
		}

		[Test]
		public void Unless_works_oposite_when_unless()
		{
			var t = new TestObject();

			new StatefulWorkflow<TestObject>()
				.Unless(x => false)
				.Yield("didn't stop")
				.Start(t);

			t.ID.ShouldBe("didn't stop");
		}

	}
}

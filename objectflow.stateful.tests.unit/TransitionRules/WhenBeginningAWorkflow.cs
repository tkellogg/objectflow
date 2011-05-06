using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Rainbow.ObjectFlow.Stateful.tests.TransitionRules
{

	#region types used for testing
	class TestObject : IStatefulObject
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
	#endregion

	[TestFixture]
	class WhenBeginningAWorkflow
	{
		private DefaultTransitionRule<TestObject> sut;
		private TestObject obj;

		[SetUp]
		public void Given()
		{
			sut = new DefaultTransitionRule<TestObject>("workflow");
			obj = new TestObject();
		}

		[Observation]
		public void NotInWorkflowYet()
		{
			Assert.False(sut.IsInWorkflow(obj));
		}

		[Observation]
		public void CanBegin()
		{
			sut.Begin(obj, "end");
			Assert.That(obj.State, Is.EqualTo("end"));
			Assert.That(sut.IsInWorkflow(obj));
		}
	}
}

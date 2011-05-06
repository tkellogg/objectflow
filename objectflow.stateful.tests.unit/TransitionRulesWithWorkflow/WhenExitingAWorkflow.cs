using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;

namespace Rainbow.ObjectFlow.Stateful.tests.TransitionRulesWithWorkflow
{
	[TestFixture]
	[Category("Integration")]
	class WhenExitingAWorkflow
	{
		private IStatefulWorkflow<TestObject> workflow;
		private TestObject data;
		private Mock<DefaultTransitionRule<TestObject>> ruleMock;
		private DefaultTransitionRule<TestObject> rule;

		[SetUp]
		public void Given()
		{
			ruleMock = new Mock<DefaultTransitionRule<TestObject>>("workflow");
			ruleMock.CallBase = true;
			rule = ruleMock.Object;
			workflow = new StatefulWorkflow<TestObject>("workflow", null, rule);
			TestFixtures.SetupWorkflow(workflow);
			data = new TestObject() { State = "Last" };
		}

		[Observation]
		public void IsInitiallyInWorkflow()
		{
			Assert.True(rule.IsInWorkflow(data));
		}

		[Observation]
		public void CanExitWorkflow()
		{
			workflow.Start(data);
			Assert.False(rule.IsInWorkflow(data));
			Assert.That(data.State, Is.Null);
			ruleMock.Verify(x => x.End(data));
		}
	}
}

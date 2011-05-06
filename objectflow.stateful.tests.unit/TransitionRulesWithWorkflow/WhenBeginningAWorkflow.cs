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
	class WhenBeginningAWorkflow
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
			data = new TestObject();
		}

		[Observation]
		public void IsntInitiallyInWorkflow()
		{
			Assert.False(rule.IsInWorkflow(data));
		}

		[Observation]
		public void CanEnterWorkflow()
		{
			workflow.Start(data);
			Assert.True(rule.IsInWorkflow(data));
			Assert.That(data.State, Is.EqualTo("First"));
			ruleMock.Verify(x => x.Begin(data, "First"));
		}
	}
}

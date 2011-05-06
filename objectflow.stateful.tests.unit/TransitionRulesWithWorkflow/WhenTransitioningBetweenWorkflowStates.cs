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
	class WhenTransitioningBetweenWorkflowStates
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
			data = new TestObject() { State = "First" };
		}

		[Observation]
		public void IsInitiallyInWorkflow()
		{
			Assert.True(rule.IsInWorkflow(data));
		}

		[Observation]
		public void CanTransitionBetweenSteps()
		{
			workflow.Start(data);
			Assert.True(rule.IsInWorkflow(data));
			Assert.That(data.State, Is.EqualTo("Last"));
			ruleMock.Verify(x => x.Transition(data, "Last"));
		}
	}
}

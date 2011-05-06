using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Rainbow.ObjectFlow.Stateful.tests.TransitionRules
{
	[TestFixture]
	class WhenEndingAWorkflow
	{
		private DefaultTransitionRule<TestObject> sut;
		private TestObject obj;

		[SetUp]
		public void Given()
		{
			sut = new DefaultTransitionRule<TestObject>("workflow");
			obj = new TestObject() { State = "begin" };
		}

		[Observation]
		public void StartsInWorkflow()
		{
			Assert.True(sut.IsInWorkflow(obj));
		}

		[Observation]
		public void CanExitWorkflow()
		{
			sut.End(obj);
			Assert.False(sut.IsInWorkflow(obj));
			Assert.Null(sut.HasBeenInWorkflow(obj));
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Rainbow.ObjectFlow.Stateful.tests.BreakingConditions
{
	[TestFixture]
	class WhenFailing
	{
		[Observation]
		public void it_throws_a_WorkflowActionFailedException()
		{
			var wf = new StatefulWorkflow<Entity>()
				.When(x => true).Fail()
				.Yield("end");

			Assert.Throws<WorkflowActionFailedException>(() => wf.Start(new Entity()));
		}

		[Observation]
		public void it_throws_a_WorkflowActionFailedException_with_a_message()
		{
			var wf = new StatefulWorkflow<Entity>()
				.When(x => true).Fail(x => x.With("a message"))
				.Yield("end");

			try
			{
				wf.Start(new Entity());
				Assert.Fail("an exception should have been thrown");
			}
			catch (WorkflowActionFailedException e)
			{
				e.Message.ShouldBe("a message");
			}
		}

		[Observation]
		public void it_throws_a_WorkflowActionFailedException_with_a_formatted_message()
		{
			var wf = new StatefulWorkflow<Entity>()
				.When(x => true).Fail(x => x.With("{0} message", 1))
				.Yield("end");

			try
			{
				wf.Start(new Entity());
				Assert.Fail("an exception should have been thrown");
			}
			catch (WorkflowActionFailedException e)
			{
				e.Message.ShouldBe("1 message");
			}
		}

		class UserDefinedException : Exception { }

		[Observation]
		public void it_throws_the_exception_created_by_the_user()
		{
			var ex = new UserDefinedException();
			var wf = new StatefulWorkflow<Entity>()
				.When(x => true).Fail(x => x.With(ex))
				.Yield("end");

			try
			{
				wf.Start(new Entity());
				Assert.Fail("an exception should have been thrown");
			}
			catch (UserDefinedException e)
			{
				Assert.ReferenceEquals(ex, e);
			}
		}
	}
}

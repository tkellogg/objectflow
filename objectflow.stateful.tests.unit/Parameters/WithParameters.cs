using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Rainbow.ObjectFlow.Helpers;

namespace Rainbow.ObjectFlow.Stateful.tests.Parameters
{
	public class WithParameters : Specification
	{
		#region Types used for testing
		public interface ITest : IStatefulObject
		{
			void Ping(int id);
		}
		#endregion

		[Observation]
		public void Operations_work_when_using_dictionary()
		{
			var wf = new StatefulWorkflow<ITest>("wf");
			wf.Yield(13);
			wf.Do((x, i) => { x.Ping((int)i["i"]); });

			var test = Mock.Of<ITest>(x => (int)x.GetStateId("wf") == 13);
			var mock = Mock.Get(test);
			wf.StartWithParams(test, new Dictionary<string, object>() { { "i", 42 } });
			mock.Verify(x => x.SetStateId("wf", null));
			mock.Verify(x => x.Ping((int)42));
		}

		[Observation]
		public void Operations_work_when_using_objects()
		{
			var wf = new StatefulWorkflow<ITest>("wf");
			wf.Yield(13);
			wf.Do((x, i) => { x.Ping((int)i["i"]); });

			var test = Mock.Of<ITest>(x => (int)x.GetStateId("wf") == 13);
			var mock = Mock.Get(test);
			wf.StartWithParams(test, new { i = 42 });
			mock.Verify(x => x.SetStateId("wf", null));
			mock.Verify(x => x.Ping((int)42));
		}

		[Observation]
		public void it_can_use_params_at_any_step()
		{
			string secondStep = null;
			var wf = new StatefulWorkflow<ITest>("wf");
			wf.Yield(13);
			wf.Do((x, i) => { x.Ping((int)i["i"]); });
			wf.Do((x, opts) => secondStep = (string)opts["o"]);

			var test = Mock.Of<ITest>(x => (int)x.GetStateId("wf") == 13);
			var mock = Mock.Get(test);
			wf.StartWithParams(test, new { i = 42, o = "hello" });
			mock.Verify(x => x.SetStateId("wf", null));
			mock.Verify(x => x.Ping((int)42));
			secondStep.ShouldBe("hello");
		}

		[Observation]
		public void branches_work_with_parameters()
		{
			var jump = Declare.Step();

			var wf = new StatefulWorkflow<ITest>("wf")
				.Unless((x, opts) => (bool)opts["flag"]).BranchTo(jump)
				.Yield("it didn't jump")
				.Define(jump)
				.Yield("it jumped");

			var test = Mock.Of<ITest>();
			wf.StartWithParams(test, new { flag = false });
			Mock.Get(test).Verify(x => x.SetStateId("wf", "it jumped"));
			Mock.Get(test).Verify(x => x.SetStateId("wf", "it didn't jump"), Times.Never());
		}

		[Observation]
		public void failed_branches_work_with_parameters()
		{
			var jump = Declare.Step();

			var wf = new StatefulWorkflow<ITest>("wf")
				.Unless((x, opts) => (bool)opts["flag"]).BranchTo(jump)
				.Yield("it didn't jump")
				.Define(jump)
				.Yield("it jumped");

			var test = Mock.Of<ITest>();
			wf.StartWithParams(test, new { flag = true });
			Mock.Get(test).Verify(x => x.SetStateId("wf", "it jumped"), Times.Never());
			Mock.Get(test).Verify(x => x.SetStateId("wf", "it didn't jump"));
		}

		[Observation]
		public void negative_branches_work_with_parameters()
		{
			var jump = Declare.Step();

			var wf = new StatefulWorkflow<ITest>("wf")
				.When((x, opts) => (bool)opts["flag"]).BranchTo(jump)
				.Yield("it didn't jump")
				.Define(jump)
				.Yield("it jumped");

			var test = Mock.Of<ITest>();
			wf.StartWithParams(test, new { flag = true });
			Mock.Get(test).Verify(x => x.SetStateId("wf", "it jumped"));
			Mock.Get(test).Verify(x => x.SetStateId("wf", "it didn't jump"), Times.Never());
		}

		[Observation]
		public void negative_failed_branches_work_with_parameters()
		{
			var jump = Declare.Step();

			var wf = new StatefulWorkflow<ITest>("wf")
				.When((x, opts) => (bool)opts["flag"]).BranchTo(jump)
				.Yield("it didn't jump")
				.Define(jump)
				.Yield("it jumped");

			var test = Mock.Of<ITest>();
			wf.StartWithParams(test, new { flag = false });
			Mock.Get(test).Verify(x => x.SetStateId("wf", "it jumped"), Times.Never());
			Mock.Get(test).Verify(x => x.SetStateId("wf", "it didn't jump"));
		}
	}
}

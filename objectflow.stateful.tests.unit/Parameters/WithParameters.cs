using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;

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
		public void OperationsWork()
		{
			var wf = new StatefulWorkflow<ITest>("wf");
			wf.Yield(13);
			wf.Do((x, i) => { x.Ping((int)i["i"]); });

			var test = Mock.Of<ITest>(x => (int)x.GetStateId("wf") == 13);
			var mock = Mock.Get(test);
			wf.StartWithParams(test, new { i = 42 }.ToDictionary());
			mock.Verify(x => x.SetStateId("wf", null));
			mock.Verify(x => x.Ping((int)42));
		}
	}
}

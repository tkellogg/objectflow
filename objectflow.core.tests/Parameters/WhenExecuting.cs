using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rainbow.ObjectFlow.Engine;

namespace Objectflow.core.tests.Parameters
{
	class WhenExecuting : Specification
	{
		IDictionary<string, object> SetParams(object val)
		{
			return new Dictionary<string, object>() { { "y", val } };
		}

		[Observation]
		public void it_returns_input_if_no_return_type()
		{
			bool wasCalled = false;
			Action<string, IDictionary<string, object>> body = (x, y) => { wasCalled = true; };
			var sut = new ParameterizedFunctionInvoker<string>(body);
			sut.SetParameters(SetParams(42));
			var result = sut.Execute("hello world");
			Assert.That(result, Is.EqualTo("hello world"));
			Assert.That(wasCalled);
		}

		[Observation]
		public void it_returns_param1_for_correct_input()
		{
			bool wasCalled = false;
			Func<string, IDictionary<string, object>, string> body = (x, y) => { wasCalled = true; return "foo"; };
			var sut = new ParameterizedFunctionInvoker<string>(body);
			sut.SetParameters(SetParams(42));
			var result = sut.Execute("bar");
			Assert.That(result, Is.EqualTo("foo"));
			Assert.That(wasCalled);
		}
	}
}

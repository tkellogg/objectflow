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
		[Observation]
		public void ReturnsInputIfNoReturnType()
		{
			bool wasCalled = false;
			Action<string, int> body = (x, y) => { wasCalled = true; };
			var sut = new ParameterizedFunctionInvoker<string>(body);
			sut.SetParameters(42);
			var result = sut.Execute("hello world");
			Assert.That(result, Is.EqualTo("hello world"));
			Assert.That(wasCalled);
		}

		[Observation]
		public void ReturnsParam1ForCorrectInput()
		{
			bool wasCalled = false;
			Func<string, int, string> body = (x, y) => { wasCalled = true; return "foo"; };
			var sut = new ParameterizedFunctionInvoker<string>(body);
			sut.SetParameters(42);
			var result = sut.Execute("bar");
			Assert.That(result, Is.EqualTo("foo"));
			Assert.That(wasCalled);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rainbow.ObjectFlow.Engine;

namespace Objectflow.core.tests.Parameters
{
	class WhenSettingParameters : Specification
	{
		private ParameterizedFunctionInvoker<string> sut;

		[SetUp]
		public void Given()
		{
			Action<string, int> body = (x, y) => { };
			sut = new ParameterizedFunctionInvoker<string>(body);
		}

		[Observation]
		public void SettingTooFewParamsThrows()
		{
			Assert.Throws<ArgumentException>(() => sut.SetParameters());
		}

		[Observation]
		public void SettingTooManyParamsThrows()
		{
			Assert.Throws<ArgumentException>(() => sut.SetParameters(42, 38));
		}

		[Observation]
		public void SettingWrongTypesThrows()
		{
			Assert.Throws<ArgumentException>(() => sut.SetParameters("dilligently"));
		}

		[Observation]
		public void SettingCorrectParamsWorks()
		{
			sut.SetParameters(42);
			Assert.That(sut.Parameters, Is.EquivalentTo(new object[] { null, 42 }));
		}
	}
}

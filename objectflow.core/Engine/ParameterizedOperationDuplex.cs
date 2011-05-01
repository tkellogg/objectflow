using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Engine
{
	internal class ParameterizedOperationDuplex<T> : OperationDuplex<T>
	{
		public ParameterizedOperationDuplex(ParameterizedFunctionInvoker<T> command)
			:base(command)
		{
		}

		public void SetParameters(object[] parameters)
		{
			((ParameterizedFunctionInvoker<T>)_command).SetParameters(parameters);
		}
	}
}

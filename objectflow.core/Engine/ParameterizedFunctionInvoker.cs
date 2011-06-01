using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Engine
{
	internal class ParameterizedFunctionInvoker<T> : MethodInvoker<T>
	{
		private readonly Delegate _body;
		internal IDictionary<string, object> Parameters { get; private set; }

		public ParameterizedFunctionInvoker(Delegate body)
		{
			_body = body;
		}

		public virtual void SetParameters(IDictionary<string, object> parameters)
		{
			Parameters = parameters;
		}

		public override T Execute(T data)
		{
			var result = _body.DynamicInvoke(data, Parameters);
			if (_body.Method.ReturnType == typeof(void))
				return data;
			else
				return (T)result;
		}
	}
}

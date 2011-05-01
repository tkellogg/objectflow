using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Engine
{
	internal class ParameterizedFunctionInvoker<T> : MethodInvoker<T>
	{
		private readonly Delegate _body;
		internal object[] Parameters { get; private set; }

		public ParameterizedFunctionInvoker(Delegate body)
		{
			_body = body;
		}

		public virtual void SetParameters(params object[] parameters)
		{
			var ptypes = _body.Method.GetParameters().Select(x => x.ParameterType).ToArray();
			if (parameters.Length == ptypes.Length - 1)
			{
				var candidate = new object[parameters.Length + 1];
				for (int i = 1; i < candidate.Length; i++)
				{
					if (ptypes[i].IsInstanceOfType(parameters[i - 1]))
						candidate[i] = parameters[i - 1];
					else
						FormatException(parameters, ptypes);
				}
				Parameters = candidate;
			}
			else
				FormatException(parameters, ptypes);
		}

		private static void FormatException(object[] parameters, Type[] ptypes)
		{
			var types = string.Join(",", ptypes.Skip(1).Select(x => x.Name).ToArray());
			var actual = string.Join(",", parameters.Select(x => x.GetType().Name).ToArray());
			throw new ArgumentException(string.Format("Expected ({0}) but got ({1})", types, actual));
		}

		public override T Execute(T data)
		{
			Parameters[0] = data;
			var result = _body.DynamicInvoke(Parameters);
			if (_body.Method.ReturnType == typeof(void))
				return data;
			else
				return (T)result;
		}
	}
}

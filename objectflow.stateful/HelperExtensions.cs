using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Stateful
{
	/// <summary>
	/// Convenient extention methods
	/// </summary>
	public static class HelperExtensions
	{
		/// <summary>
		/// Convert all properties to a dictionary of strings with values
		/// </summary>
		/// <param name="self"></param>
		/// <returns></returns>
		public static IDictionary<string, object> ToDictionary(this object self)
		{
			var ret = new Dictionary<string, object>();

			foreach (var prop in self.GetType().GetProperties())
			{
				var val = prop.GetValue(self, null);
				ret[prop.Name] = val;
			}

			return ret;
		}
	}
}

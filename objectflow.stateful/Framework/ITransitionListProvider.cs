using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Stateful.Framework
{
	/// <summary>
	/// 
	/// </summary>
	public interface ITransitionListProvider
	{
		/// <summary>
		/// Gets a list of transitions that are allowed for the current user
		/// </summary>
		/// <returns></returns>
		IList<ITransition> GetList();
	}
}

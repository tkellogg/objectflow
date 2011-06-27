using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using Rainbow.ObjectFlow.Helpers;

namespace Rainbow.ObjectFlow.Stateful.tests.TransitionGateways
{
	#region Types used for testing
	public class TBasic : IStatefulObject
	{
		private object state = "start";
		public object GetStateId(object workflowId)
		{
			return state;
		}
		public void SetStateId(object workflowId, object stateId)
		{
			state = stateId;
		}
	}

	public class TIdentifiable : TBasic
	{
		public override bool Equals(object obj)
		{
			var t = (TBasic)obj;
			return GetStateId(null) == t.GetStateId(null);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				if (GetStateId(null) != null)
					return GetStateId(null).GetHashCode() * 31;
				else return 0;
			}
		}
	}
	#endregion

	[TestFixture(TypeArgs = new[]{typeof(TBasic)})]
	[TestFixture(typeof(TIdentifiable))]
	class WhenChangingStateMidTransition<T>
		where T : class, IStatefulObject, new()
	{


		[Observation]
		public void ItUsesTheOriginalStateWhenCheckingSecurity()
		{
			var transitions = new[] { new Transition(null, "start", "end") };
			var security = new DefaultTransitionGateway(transitions);

			var end = Declare.Step();
			var wf = new StatefulWorkflow<T>(null, security)
				.Define(end)
				.Yield("end")
				.Yield("start")
				.Do(x => x.SetStateId(null, "end"))
				.When(x => false, otherwise: end);

			var t = new T();
			wf.Start(t);

			// Main assertion is really that it didn't throw a security exception
			Assert.That(t.GetStateId(null), Is.EqualTo("end"));
		}
	}
}

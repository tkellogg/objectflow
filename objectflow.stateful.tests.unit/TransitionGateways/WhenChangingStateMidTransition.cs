using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using Rainbow.ObjectFlow.Helpers;

namespace Rainbow.ObjectFlow.Stateful.tests.TransitionGateways
{
	[TestFixture]
	class WhenChangingStateMidTransition
	{

		#region Types used for testing
		class T : IStatefulObject
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
		#endregion

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

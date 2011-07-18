using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rainbow.ObjectFlow.Stateful.Components;

namespace Rainbow.ObjectFlow.Stateful.tests.Components.SecurityGateways
{
	[TestFixture]
	class WhenUsingRelaxedSecurity
	{
		private Transition[] _transitions;
		private RelaxedTransitionGateway sut;

		[SetUp]
		public void SetUp()
		{
			_transitions = new[] { new Transition(1, 1, 2), new Transition(1, 1, 3), new Transition(1, 1, 4), new Transition(2, 1, 4) };
			sut = new RelaxedTransitionGateway(1);
			sut.TransitionList = () => _transitions;
		}

		[Observation]
		public void it_only_allows_single_whitelisted_items()
		{
			sut.TransitionList = () => new[] { new Transition(1, 1, 2) };
			var result = sut.AllowTransitions(_transitions);
			Assert.That(result.ToList(), Is.EquivalentTo(new[] { new Transition(1, 1, 3), new Transition(1, 1, 4) }));
		}

		[Observation]
		public void empty_blacklist_allows_everything()
		{
			sut.TransitionList = () => new ITransition[0];
			var result = sut.AllowTransitions(_transitions);
			Assert.That(result.ToList(), Is.EquivalentTo(_transitions.Where(x => object.Equals(x.WorkflowId, 1)).ToList()));
		}

		[Observation]
		public void it_only_allows_transitions_from_current_workflow()
		{
			sut.TransitionList = () => new[] { new Transition(1, 1, 2), new Transition(2, 1, 4) };
			var result = sut.AllowTransitions(_transitions);
			Assert.That(result.ToList(), Is.EquivalentTo(new[] { new Transition(1, 1, 3) }));
		}

	}
}

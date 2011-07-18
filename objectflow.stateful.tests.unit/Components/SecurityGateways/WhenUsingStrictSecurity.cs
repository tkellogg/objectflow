using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rainbow.ObjectFlow.Stateful.Components;

namespace Rainbow.ObjectFlow.Stateful.tests.Components.SecurityGateways
{
	[TestFixture]
	class WhenUsingStrictSecurity
	{
		private Transition[] _transitions;
		private StrictTransitionGateway sut;

		[SetUp]
		public void SetUp()
		{
			_transitions = new[] { new Transition(1, 1, 2), new Transition(1, 1, 3), new Transition(1, 1, 4), new Transition(2, 1, 4) };
			sut = new StrictTransitionGateway(1);
			sut.TransitionList = () => _transitions;
		}

		[Observation]
		public void it_only_allows_single_whitelisted_items()
		{
			var result = sut.AllowTransitions(new[] { new Transition(1, 1, 2) });
			Assert.That(result.ToList(), Is.EquivalentTo(new[] { new Transition(1, 1, 2) }));
		}

		[Observation]
		public void empty_whitelist_denies_everything()
		{
			var result = sut.AllowTransitions(new ITransition[] {  });
			Assert.That(result.ToList(), Is.Empty);
		}

		[Observation]
		public void it_only_allows_transitions_from_current_workflow()
		{
			var result = sut.AllowTransitions(new[] { new Transition(1, 1, 2), new Transition(2, 1, 4) });
			Assert.That(result.ToList(), Is.EquivalentTo(new[] { new Transition(1, 1, 2) }));
		}

	}
}

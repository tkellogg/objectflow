using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Rainbow.ObjectFlow.Helpers;

namespace Rainbow.ObjectFlow.Stateful.tests.TransitionGateways
{
    public class GivenOnlyBranches : Specification
    {
        Mock<ITransitionGateway> gateway = new Mock<ITransitionGateway>();

        #region Types for testing
        class StatefulObject : IStatefulObject
        {
            int? state;
            #region IStatefulObject Members

            public object GetStateId(object workflowId)
            {
                return state;
            }

            public void SetStateId(object workflowId, object stateId)
            {
                state = (int?)stateId;
            }

            #endregion
        }
        #endregion

        [Observation]
        public void NoBranchesHasOneTransition()
        {
            var wf = new StatefulWorkflow<StatefulObject>(2, gateway.Object)
                .Do(x => x);

            var transitions = wf.PossibleTransitions.ToList();
            transitions.Count.ShouldBe(1);
            transitions.Where(x => x.From == null && x.To == null).Count().ShouldBe(1);
        }

        [Observation]
        public void OneBranchHasOneTransition()
        {
            var branch1 = Declare.Step();
            var wf = new StatefulWorkflow<StatefulObject>(2, gateway.Object)
                .Define(defineAs: branch1)
                .Do(x => x)
                .When(x => true, otherwise: branch1);

            var transitions = wf.PossibleTransitions.ToList();
            transitions.Count.ShouldBe(1);
            transitions.Where(x => x.From == null && x.To == null).Count().ShouldBe(1);
        }

        [Observation]
        public void TwoBranchesHasOneTransition()
        {
            var branch1 = Declare.Step();
            var wf = new StatefulWorkflow<StatefulObject>(2, gateway.Object)
                .Define(defineAs: branch1)
                .When(x => true, otherwise: branch1)
                .Do(x => x)
                .When(x => true, otherwise: branch1);

            var transitions = wf.PossibleTransitions.ToList();
            transitions.Count.ShouldBe(1);
            transitions.Where(x => x.From == null && x.To == null).Count().ShouldBe(1);
        }

        [Observation]
        public void TwoForwardReferencesHasOneTransition()
        {
            var branch1 = Declare.Step();
            var wf = new StatefulWorkflow<StatefulObject>(2, gateway.Object)
                .When(x => true, otherwise: branch1)
                .Do(x => x)
                .When(x => true, otherwise: branch1)
                .Define(defineAs: branch1);

            var transitions = wf.PossibleTransitions.ToList();
            transitions.Count.ShouldBe(1);
            transitions.Where(x => x.From == null && x.To == null).Count().ShouldBe(1);
        }
    }
}

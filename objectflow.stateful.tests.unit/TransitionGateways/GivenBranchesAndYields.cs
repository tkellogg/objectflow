using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Rainbow.ObjectFlow.Helpers;

namespace Rainbow.ObjectFlow.Stateful.tests.TransitionGateways
{
    public class GivenBranchesAndYields : Specification
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
        public void SingleBranchBetweenFlowsAddsATransition()
        {
            var branch1 = Declare.Step();
            var wf = new StatefulWorkflow<StatefulObject>(2, gateway.Object)
                .Yield(1)
                .Define(defineAs: branch1)
                .Yield(2)
                .When(x => true, otherwise: branch1);

            var transitions = wf.PossibleTransitions.ToList();
            transitions.Count.ShouldBe(4);
            transitions.Where(x => x.From == null && (int?)x.To == 1).Count().ShouldBe(1);
            transitions.Where(x => (int?)x.From == 1 && (int?)x.To == 2).Count().ShouldBe(1);
            transitions.Where(x => (int?)x.From == 2 && x.To == null).Count().ShouldBe(1);
            // as well as
            transitions.Where(x => (int?)x.From == 2 && (int?)x.To == 1).Count().ShouldBe(1);
        }

        [Observation]
        public void SingleBranchBetweenFlowsAddsATransition_ForwardReference()
        {
            var branch1 = Declare.Step();
            var wf = new StatefulWorkflow<StatefulObject>(2, gateway.Object)
                .Yield(1)
                .When(x => true, otherwise: branch1)
                .Yield(2)
                .Define(defineAs: branch1);

            var transitions = wf.PossibleTransitions.ToList();
            transitions.Count.ShouldBe(4);
            transitions.Where(x => x.From == null && (int?)x.To == 1).Count().ShouldBe(1);
            transitions.Where(x => (int?)x.From == 1 && (int?)x.To == 2).Count().ShouldBe(1);
            transitions.Where(x => (int?)x.From == 2 && x.To == null).Count().ShouldBe(1);
            // as well as
            transitions.Where(x => (int?)x.From == 2 && (int?)x.To == 1).Count().ShouldBe(1);
        }
    }
}

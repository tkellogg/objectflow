using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using Rainbow.ObjectFlow.Helpers;

namespace Rainbow.ObjectFlow.Stateful.tests.PossibleTransitions
{
    [TestFixture]
    public class GivenOnlyYields
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
        public void NoYieldsHasOneTransition()
        {
            var wf = new StatefulWorkflow<StatefulObject>(2, gateway.Object)
                .Do(x => x);

            var transitions = wf.PossibleTransitions.ToList();
            transitions.Count.ShouldBe(1);
            transitions.Where(x => x.From == null && x.To == null).Count().ShouldBe(1);
        }

        [Observation]
        public void OneYieldHasTwoTransitions()
        {
            var wf = new StatefulWorkflow<StatefulObject>(2, gateway.Object)
                .Do(x => x)
                .Yield(1);

            var transitions = wf.PossibleTransitions.ToList();
            transitions.Count.ShouldBe(2);
            transitions.Where(x => x.From == null && (int?)x.To == 1).Count().ShouldBe(1);
            transitions.Where(x => (int?)x.From == 1 && x.To == null).Count().ShouldBe(1);
        }

        [Observation]
        public void TwoYieldsHasThreeTransitions()
        {
            var wf = new StatefulWorkflow<StatefulObject>(2, gateway.Object)
                .Yield(1)
                .Do(x => x)
                .Yield(2);

            var transitions = wf.PossibleTransitions.ToList();
            transitions.Count.ShouldBe(3);
            transitions.Where(x => x.From == null && (int?)x.To == 1).Count().ShouldBe(1);
            transitions.Where(x => (int?)x.From == 1 && (int?)x.To == 2).Count().ShouldBe(1);
            transitions.Where(x => (int?)x.From == 2 && x.To == null).Count().ShouldBe(1);
        }

        [Observation]
        public void StartsAndEndsWithDefines()
        {
            var start = Declare.Step();
            var end = Declare.Step();
            var wf = new StatefulWorkflow<StatefulObject>(2, gateway.Object)
                .Define(start)
                .Yield(1)
                .Define(end)
                ;

            var transitions = wf.PossibleTransitions.ToList();
            transitions.Count.ShouldBe(2);
            transitions.Where(x => x.From == null && (int?)x.To == 1).Count().ShouldBe(1);
            transitions.Where(x => (int?)x.From == 1 && x.To == null).Count().ShouldBe(1);
        }
    }
}

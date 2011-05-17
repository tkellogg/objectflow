using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Rainbow.ObjectFlow.Helpers;
using NUnit.Framework;

namespace Rainbow.ObjectFlow.Stateful.tests.PossibleTransitions
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

        #region Back references

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
			transitions.Should(Have.Count.EqualTo(4));
			
			transitions.Should(Have.Some.Matches<ITransition>(x => x.From == null && (int?)x.To == 1));
            transitions.Should(Have.Some.Matches<ITransition>(x => (int?)x.From == 1 && (int?)x.To == 2));
            transitions.Should(Have.Some.Matches<ITransition>(x => (int?)x.From == 2 && x.To == null));
            // as well as
			transitions.Should(Have.Some.Matches<ITransition>(x => (int?)x.From == 2 && (int?)x.To == 2));
			transitions.Should(Have.None.Matches<ITransition>(x => (int?)x.From == 2 && (int?)x.To == 1));
        }

        [Observation]
        public void SingleBranchBetweenFlowsAddsATransition_2ndArrangement()
        {
            var branch1 = Declare.Step();
            var wf = new StatefulWorkflow<StatefulObject>(2, gateway.Object)
                .Define(defineAs: branch1)
                .Yield(1)
                .When(x => true, otherwise: branch1)
                .Yield(2)
                ;

			var transitions = wf.PossibleTransitions.ToList();
			transitions.Should(Have.Count.EqualTo(4));

            transitions.Where(x => x.From == null && (int?)x.To == 1).Count().ShouldBe(1);
            transitions.Where(x => (int?)x.From == 1 && (int?)x.To == 2).Count().ShouldBe(1);
            transitions.Where(x => (int?)x.From == 2 && x.To == null).Count().ShouldBe(1);
            // as well as
			transitions.Where(x => (int?)x.From == 1 && (int?)x.To == null).Count().ShouldBe(0);
			transitions.Where(x => (int?)x.From == 1 && (int?)x.To == 1).Count().ShouldBe(1);
        }

        [Observation]
        public void DoubleBranchDoesNotAddDuplicate()
        {
            var branch1 = Declare.Step();
            var wf = new StatefulWorkflow<StatefulObject>(2, gateway.Object)
                .Yield(1)
                .Define(defineAs: branch1)
                .Yield(2)
                .When(x => true, otherwise: branch1)
                .When(x => true, otherwise: branch1);

			var transitions = wf.PossibleTransitions.ToList();
			transitions.Should(Have.Count.EqualTo(4));

            transitions.Where(x => x.From == null && (int?)x.To == 1).Count().ShouldBe(1);
            transitions.Where(x => (int?)x.From == 1 && (int?)x.To == 2).Count().ShouldBe(1);
            transitions.Where(x => (int?)x.From == 2 && x.To == null).Count().ShouldBe(1);
            // as well as
			transitions.Where(x => (int?)x.From == 2 && (int?)x.To == 1).Count().ShouldBe(0);
			transitions.Where(x => (int?)x.From == 2 && (int?)x.To == 2).Count().ShouldBe(1);
        }

        [Observation]
        public void DoubleDefineDoesNotAddDuplicate()
        {
            var branch1 = Declare.Step();
            var branch2 = Declare.Step();
            var wf = new StatefulWorkflow<StatefulObject>(2, gateway.Object)
                .Yield(1)
                .Define(defineAs: branch1)
                .Define(defineAs: branch2)
                .Yield(2)
                .When(x => true, otherwise: branch1)
                .When(x => true, otherwise: branch2);

			var transitions = wf.PossibleTransitions.ToList();
			transitions.Should(Have.Count.EqualTo(4));

            transitions.Where(x => x.From == null && (int?)x.To == 1).Count().ShouldBe(1);
            transitions.Where(x => (int?)x.From == 1 && (int?)x.To == 2).Count().ShouldBe(1);
            transitions.Where(x => (int?)x.From == 2 && x.To == null).Count().ShouldBe(1);
            // as well as
			transitions.Where(x => (int?)x.From == 2 && (int?)x.To == 1).Count().ShouldBe(0);
			transitions.Where(x => (int?)x.From == 2 && (int?)x.To == 2).Count().ShouldBe(1);
        }
        #endregion

        #region Forward references

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
			transitions.Should(Have.Count.EqualTo(4));

            transitions.Where(x => x.From == null && (int?)x.To == 1).Count().ShouldBe(1);
            transitions.Where(x => (int?)x.From == 1 && (int?)x.To == 2).Count().ShouldBe(1);
            transitions.Where(x => (int?)x.From == 2 && x.To == null).Count().ShouldBe(1);
            // as well as
			transitions.Where(x => (int?)x.From == 2 && (int?)x.To == 1).Count().ShouldBe(0);
			transitions.Where(x => (int?)x.From == 1 && (int?)x.To == null).Count().ShouldBe(1);
        }

        [Observation]
        public void DoubleBranchDoesNotAddDuplicate_ForwardReference()
        {
            var branch1 = Declare.Step();
            var wf = new StatefulWorkflow<StatefulObject>(2, gateway.Object)
                .Yield(1)
                .When(x => true, otherwise: branch1)
                .When(x => true, otherwise: branch1)
                .Yield(2)
                .Define(defineAs: branch1)
                ;

			var transitions = wf.PossibleTransitions.ToList();
			transitions.Should(Have.Count.EqualTo(4));

            transitions.Where(x => x.From == null && (int?)x.To == 1).Count().ShouldBe(1);
            transitions.Where(x => (int?)x.From == 1 && (int?)x.To == 2).Count().ShouldBe(1);
            transitions.Where(x => (int?)x.From == 2 && x.To == null).Count().ShouldBe(1);
            // as well as
			transitions.Where(x => (int?)x.From == 2 && (int?)x.To == 1).Count().ShouldBe(0);
			transitions.Where(x => (int?)x.From == 1 && (int?)x.To == null).Count().ShouldBe(1);
        }

        [Observation]
        public void DoubleDefineDoesNotAddDuplicate_ForwardReference()
        {
            var branch1 = Declare.Step();
            var branch2 = Declare.Step();
            var wf = new StatefulWorkflow<StatefulObject>(2, gateway.Object)
                .Yield(1)
                .When(x => true, otherwise: branch1)
                .When(x => true, otherwise: branch2)
                .Yield(2)
                .Define(defineAs: branch1)
                .Define(defineAs: branch2)
                ;

			var transitions = wf.PossibleTransitions.ToList();
			transitions.Should(Have.Count.EqualTo(4));

            transitions.Where(x => x.From == null && (int?)x.To == 1).Count().ShouldBe(1);
            transitions.Where(x => (int?)x.From == 1 && (int?)x.To == 2).Count().ShouldBe(1);
            transitions.Where(x => (int?)x.From == 2 && x.To == null).Count().ShouldBe(1);
            // as well as
			transitions.Where(x => (int?)x.From == 2 && (int?)x.To == 1).Count().ShouldBe(0);
			transitions.Where(x => (int?)x.From == 1 && (int?)x.To == null).Count().ShouldBe(1);
        }
        #endregion
    }
}

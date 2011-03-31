using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Rainbow.ObjectFlow.Helpers;

namespace Rainbow.ObjectFlow.Stateful.tests.TransitionGateways
{
    public class WhenTransitionIsBlocked : Specification
    {
        Mock<Gateway> gateway;
        private Mock<StatefulObject> obj;

        #region Types for testing
        public abstract class StatefulObject : IStatefulObject
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

            public abstract StatefulObject Feedback(string msg);
        }

        public abstract class Gateway : ITransitionGateway
        {
            public IEnumerable<ITransition> AllowTransitions(IList<ITransition> transitions)
            {
                foreach (var t in transitions)
                    if (IsTransitionAllowed(t))
                        yield return t;
            }

            public abstract bool IsTransitionAllowed(ITransition transition);
        }
        #endregion

        [Scenario]
        public void Given()
        {
            gateway = new Mock<Gateway>();
            gateway.SetReturnsDefault(true);
            obj = new Mock<StatefulObject>();
            obj.SetReturnsDefault(obj.Object);
        }

        [Observation]
        public void Simple()
        {
            var wf = new StatefulWorkflow<StatefulObject>(2, gateway.Object)
                .Yield(1)
                .Do(x => x.Feedback("middle"))
                .Yield(2)
                ;
            gateway.SetReturnsDefault<bool>(true);
            gateway.Setup(x => x.IsTransitionAllowed(
                It.Is<ITransition>(y => (int?)y.From == 1 && (int?)y.To == 2)))
                .Returns(false);
            wf.Start(obj.Object);
            Assert.Throws<UnallowedTransitionException>(() => wf.Start(obj.Object));
            obj.Verify(x => x.Feedback("middle"), Times.Never());
        }

        [Observation]
        public void BranchingIntoUnallowedState()
        {
            var branch1 = Declare.Step();
            var wf = new StatefulWorkflow<StatefulObject>(2, gateway.Object)
                .Yield(1)
                .Define(branch1)
                .Do(x => x.Feedback("middle"))
                .Yield(2)
                .When(x => false, otherwise: branch1)
                ;

            gateway.SetReturnsDefault<bool>(true);
            gateway.Setup(x => x.IsTransitionAllowed(
                It.Is<ITransition>(y => object.Equals(y.From, 2) && object.Equals(y.To, 1))))
                .Returns(false);
            wf.Start(obj.Object);
            wf.Start(obj.Object);

            Assert.Throws<UnallowedTransitionException>(() => wf.Start(obj.Object));
            obj.Verify(x => x.Feedback("middle"), Times.Once()); // not twice
        }

    }
}

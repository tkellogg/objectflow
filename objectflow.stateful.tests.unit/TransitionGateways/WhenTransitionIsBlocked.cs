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
        Mock<ITransitionGateway> gateway;
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
        #endregion

        [Scenario]
        public void Given()
        {
            gateway = new Mock<ITransitionGateway>();
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

            gateway.Setup(x => x.IsTransitionAllowed(
                It.Is<ITransition>(y => object.Equals(y.From, 2) && object.Equals(y.To, 1))))
                .Returns(false);
            wf.Start(obj.Object);
            wf.Start(obj.Object);
            /* Test is failing because Dispatcher is swalling the fucking exception. This is
             * too much work for a friday afternoon when I started working at 4:30 am
             */
            Assert.Throws<UnallowedTransitionException>(() => wf.Start(obj.Object));
            obj.Verify(x => x.Feedback("middle"), Times.Once()); // not twice
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;

namespace Rainbow.ObjectFlow.Stateful.tests.PossibleTransitions
{
    public class FactoryWantsTransitions : Specification
    {
        private Factory wf;

        #region Types used for testing
        class Factory : WorkflowFactory<IStatefulObject> {
            public Mock<StatefulWorkflow<IStatefulObject>> mock;
            public List<ITransition> transisions;

            protected override IStatefulWorkflow<IStatefulObject> Define()
            {
                var g = new Mock<ITransitionGateway>();
                g.SetReturnsDefault<bool>(true);
                mock = new Mock<StatefulWorkflow<IStatefulObject>>();
                mock.SetReturnsDefault<IEnumerable<ITransition>>(transisions);
                return mock.Object;
            }
        }
        #endregion

        [SetUp]
        public void Given()
        {
            // Setup transition mocks
            var mock1 = new Mock<ITransition>();
            mock1.SetupGet(x => x.From).Returns(null);
            mock1.SetupGet(x => x.To).Returns(1);
            var mock2 = new Mock<ITransition>();
            mock2.SetupGet(x => x.From).Returns(1);
            mock2.SetupGet(x => x.To).Returns(2);
            // Setup factory mock
            wf = new Factory();
            wf.transisions = new List<ITransition>() {mock1.Object, mock2.Object};
        }

        [Observation]
        public void AndCanDealWithNullObjects()
        {
            var result = wf.GetPossibleTransitions(null);
            result.ShouldNotbeNull();
            result.Count().ShouldBe(0);
        }

        [Observation]
        public void AndCanInteractWithWorkflow()
        {
            var mock = new Mock<IStatefulObject>();
            mock.SetReturnsDefault<object>(1);
            var result = wf.GetPossibleTransitions(mock.Object);
            result.ShouldNotbeNull();
            Assert.That(result.Count(), Is.GreaterThan(0));
        }

        [Observation]
        public void AndCanDealWithNullPossibleTransitions()
        {
            wf.transisions = null;  // null PossibleTransitions
            var mock = new Mock<IStatefulObject>();
            mock.SetReturnsDefault<object>(null);
            var result = wf.GetPossibleTransitions(mock.Object);
            result.ShouldNotbeNull();
            result.Count().ShouldBe(0);
        }
    }
}

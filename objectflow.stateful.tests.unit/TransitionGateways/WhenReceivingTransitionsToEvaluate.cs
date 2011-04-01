using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;

namespace Rainbow.ObjectFlow.Stateful.tests.TransitionGateways
{
    [TestFixture]
    class WhenReceivingTransitionsToEvaluate
    {
        private IList<ITransition> result;
        
        #region Types used for testing

        class Workflow : WorkflowMediator<IStatefulObject>
        {
            public Mock<ITransitionGateway> _gateway;
            protected override IStatefulWorkflow<IStatefulObject> Define()
            {
                return new StatefulWorkflow<IStatefulObject>("workflow", _gateway.Object)
                    .Yield(1)
                    .Yield(2);
            }
        }

        #endregion

        [SetUp]
        public void Given()
        {
            var wf = new Workflow();
            wf._gateway = new Mock<ITransitionGateway>();
            wf._gateway.Setup(x => x.AllowTransitions(It.IsAny<IList<ITransition>>()))
                .Returns<IList<ITransition>>(x => x)
                .Callback<IList<ITransition>>(x => { result = x; });
            var obj = new Mock<IStatefulObject>().SetupAllProperties();
            obj.Setup(x => x.GetStateId(It.IsAny<object>())).Returns(1);

            wf.Start(obj.Object);
        }
        
        [Test]
        public void AllTransitionsMatchToFromAndWorkflowID()
        {
            Assert.That(result, Is.All.Matches<ITransition>(x =>
            {
                return (int)x.To == 2 && (int)x.From == 1 && (string)x.WorkflowId == "workflow";
            }));
        }
    }
}

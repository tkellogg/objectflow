using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Stateful;
using Moq;

namespace Objectflow.Stateful.tests.unit.StatefulWorkflows
{
    public class WhenStartingFromScratch : Specification
    {
        private IStatefulWorkflow<A1> _workflow;
        private Mock<A1> _object;
        
        #region Types used for testing

        interface A1 : IStatefulObject { A1 GotHere(string msg); }
        
        #endregion
        
        [Scenario]
        public void Given()
        {
            _workflow = new StatefulWorkflow<A1>("test");
            _workflow.Do(x => x.GotHere("starting"))
                .Yield("stop point")
                .Do(x => x.GotHere("finished"));
            _object = new Mock<A1>();
            _object.Setup(x => x.GotHere(It.IsAny<string>())).Returns(_object.Object);
        }

        [Observation]
        public void ShouldBeAbleToPauseSmoothly()
        {
            var obj = _workflow.Start();

            _object.Verify(x => x.GotHere("starting"), Times.Once());
            obj.GetStateId("test").ShouldBe("stop point");
            _object.Verify(x => x.GotHere("finished"), Times.Never());
        }

        [Observation]
        public void ShouldBeAbleToResumeSmoothly()
        {
            var obj = _workflow.Start();
            _workflow.Start(obj);

            _object.Verify(x => x.GotHere("starting"), Times.Once());
            obj.GetStateId("test").ShouldBe("stop point");
            _object.Verify(x => x.GotHere("finished"), Times.Once());
        }
    }
}

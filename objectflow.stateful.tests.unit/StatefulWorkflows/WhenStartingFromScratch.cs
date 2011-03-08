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

        public interface A1 : IStatefulObject { A1 GotHere(string msg); }
        
        #endregion
        
        [Scenario]
        public void Given()
        {
            _workflow = new StatefulWorkflow<A1>("test");
            _workflow.Do(x => x.GotHere("starting"))
                .Yield("stop point")
                .Do(x => x.GotHere("finished"));
            _object = new Mock<A1>();
            _object.Setup(x => x.GotHere("starting")).Returns(_object.Object);
            _object.Setup(x => x.GotHere("finished")).Returns(_object.Object);
        }

        [Observation]
        public void ShouldPauseWhenArrivesAtYield()
        {
            var obj = _workflow.Start(_object.Object);

            _object.Verify(x => x.GotHere("starting"), Times.Once());
            _object.Verify(x => x.SetStateId("test", "stop point"), Times.Once());
            _object.Verify(x => x.GotHere("finished"), Times.Never());
        }

        [Observation]
        public void ShouldResumeWhenGivenAValidKey()
        {
            var obj = _workflow.Start(_object.Object);
            _object.Setup(x => x.GetStateId("test")).Returns("stop point");
            _workflow.Start(obj);

            _object.Verify(x => x.GotHere("starting"), Times.Once());
            obj.GetStateId("test").ShouldBe("stop point");
            _object.Verify(x => x.GotHere("finished"), Times.Once());
        }

    }
}

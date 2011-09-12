using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Helpers;
using Moq;

namespace Objectflow.core.tests.DeclaredOperations
{
    public class WithForwardReferences : Specification
    {
        private IWorkflow<IObject> wf;
        private int counter;
        private Mock<IObject> mock;

        #region Types used for testing
        public interface IObject { IObject Feedback(string point); }
        #endregion


        [Scenario]
        public void Given()
        {
            var branchPoint = Declare.Step();
            wf = new Workflow<IObject>()
                .Do(x => x.Feedback("beginning"), If.IsTrue(() => counter < 1, branchPoint))
                .Do(x => x.Feedback("middle"))
                .Do(x => x.Feedback("end"),  branchPoint);
            mock = new Mock<IObject>();
            mock.Setup(x => x.Feedback(It.IsAny<string>())).Returns(mock.Object);
        }

        [Observation]
        public void ObservesForwardReference()
        {
            counter = 1;
            wf.Start(mock.Object);
            mock.Verify(x => x.Feedback("beginning"), Times.Never());
            mock.Verify(x => x.Feedback("middle"), Times.Never());
            mock.Verify(x => x.Feedback("end"), Times.Once());
        }
    }
}

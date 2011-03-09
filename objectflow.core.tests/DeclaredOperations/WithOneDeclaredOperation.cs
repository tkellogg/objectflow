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
    public class WithOneDeclaredOperation : Specification
    {
        private IWorkflow<IObject> w;
        private int tracker;
        private Mock<IObject> mock;
        
        #region Interfaces for mocking
        public interface IObject { IObject Feedback(string point); }
        #endregion
        
        [Scenario]
        public void Given()
        {
            IDeclaredOperation branchPoint;
            w = new Workflow<IObject>()
                .Do(x => x.Feedback("branch point"), out branchPoint)
                .Do(x => x.Feedback("first point"), If.IsTrue(() => tracker < 2, branchPoint))
                .Do(x => x.Feedback("second point"), If.IsTrue(() => tracker < 1, branchPoint))
                ;
            
            mock = new Mock<IObject>();
            mock.Setup(x => x.Feedback(It.IsAny<string>())).Returns(mock.Object);
            // This setup is to avoid infinite loops in the test data
            int timer = 0;
            mock.Setup(x => x.Feedback("branch point")).Callback(() =>
            {
                if (++timer == 2)
                    tracker = 0;
            }).Returns(mock.Object);
        }

        [Observation]
        public void ShouldHitEveryStateWhenNoConstraintsFail()
        {
            tracker = 0;
            w.Start(mock.Object);
            mock.Verify(x => x.Feedback("branch point"), Times.Once());
            mock.Verify(x => x.Feedback("first point"), Times.Once());
            mock.Verify(x => x.Feedback("second point"), Times.Once());
        }

        [Observation]
        public void ShouldHitFirstStateTwiceWhenFirstConstraintFails()
        {
            tracker = 2;
            w.Start(mock.Object);
            mock.Verify(x => x.Feedback("branch point"), Times.Exactly(2));
            mock.Verify(x => x.Feedback("first point"), Times.Once());
            mock.Verify(x => x.Feedback("second point"), Times.Once());
        }

        [Observation]
        public void ShouldHitSecondStateTwiceWhenSecondConstraintFails()
        {
            tracker = 1;
            w.Start(mock.Object);
            mock.Verify(x => x.Feedback("branch point"), Times.Exactly(2));
            mock.Verify(x => x.Feedback("first point"), Times.Exactly(2));
            mock.Verify(x => x.Feedback("second point"), Times.Once());
        }
    }
}

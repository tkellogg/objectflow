using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Helpers;
using Moq;
using Rainbow.ObjectFlow.Stateful;

namespace Objectflow.Stateful.tests.unit.DeclaredOperationsStillWork
{
    /// <summary>
    /// This test spec is very similar to a test spec in the core with some small modifications
    /// to test stateful workflows
    /// </summary>
    public class WithOneDeclaredOperation : Specification
    {
        private IStatefulWorkflow<IObject> w;
        private int tracker;
        private Mock<IObject> mock;
        
        #region Types for mocking
        public abstract class IObject : IStatefulObject { 
            public abstract IObject Feedback(string point);

            #region IStatefulObject Members

            private object state;

            public object GetStateId(object workflowId)
            {
                return state;
            }

            public void SetStateId(object workflowId, object stateId)
            {
                state = stateId;
            }

            #endregion
        }
        #endregion
        
        [Scenario]
        public void Given()
        {
            IDeclaredOperation branchPoint;
            w = new StatefulWorkflow<IObject>("test")
                .Do(x => x.Feedback("branch point"), out branchPoint)
                .Do(x => x.Feedback("first point"), If.IsTrue(() => tracker < 2, branchPoint))
                .Yield(1)
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
            w.Start(mock.Object);
            mock.Verify(x => x.Feedback("branch point"), Times.Exactly(2));
            mock.Verify(x => x.Feedback("first point"), Times.Once());
            mock.Verify(x => x.Feedback("second point"), Times.Once());
        }

        [Observation]
        public void ShouldHitSecondStateTwiceWhenSecondConstraintFails()
        {
            /* We have a real problem with stateful workflows being too complicated.
             * From it's all assuming that everything is sequential and you never go
             * backwards. But now we go backwards
             * 
             * To fix, have `Yield` actually generate a task in the chain that only
             * does `x => x.SetStateId(newStateId)`. This should actually be extremely
             * easy and should greatly simplify the code we have in StatefulWorkflow
             */
            tracker = 1;
            w.Start(mock.Object);
            w.Start(mock.Object);
            w.Start(mock.Object);
            mock.Verify(x => x.Feedback("branch point"), Times.Exactly(2));
            mock.Verify(x => x.Feedback("first point"), Times.Exactly(2));
            mock.Verify(x => x.Feedback("second point"), Times.Once());
        }
    }
}

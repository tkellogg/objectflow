using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Helpers;
using NUnit.Framework;

namespace Rainbow.ObjectFlow.Stateful.tests.PossibleTransitions
{
    public class WhenObjectCanGoManyWays : Specification
    {
        private Factory wf;

        #region Types used for testing
        class WObject : IStatefulObject
        {
            public string State { get; set; }
            public bool GoStartToMiddle { get; set; }
            public bool GoMiddleToStart { get; set; }
            public bool GoEndToStart { get; set; }
            public bool GoEndToMiddle { get; set; }

            #region IStatefulObject Members

            public object GetStateId(object workflowId)
            {
                return State;
            }

            public void SetStateId(object workflowId, object stateId)
            {
                State = (string)stateId;
            }

            #endregion
        }

        class Factory : WorkflowMediator<WObject>
        {
            protected override IStatefulWorkflow<WObject> Define()
            {
                var start = Declare.Step();
                var middle = Declare.Step();
                var wf = new StatefulWorkflow<WObject>()
                    .Define(defineAs: start)
                    .Unless(x => x.GoStartToMiddle, otherwise: middle)
                    .Yield("first")
                    .Define(defineAs: middle)
                    .Unless(x => x.GoMiddleToStart, otherwise: start)
                    .Yield("second")
                    .Unless(x => x.GoEndToMiddle, otherwise: middle)
                    .Unless(x => x.GoEndToStart, otherwise: start)
                    ;
                return wf;
            }

            protected override void DefineHints()
            {
                SetHint(null, "second", x => x.GoStartToMiddle = true);
                SetHint(null, "first", x => x.GoStartToMiddle = false);
                SetHint("first", "second", x => x.GoMiddleToStart = false);
                SetHint("first", "first", x => x.GoMiddleToStart = true);
                SetHint("second", null, x => { x.GoEndToMiddle = false; x.GoEndToStart = false; });
                SetHint("second", "second", x => { x.GoEndToMiddle = true; x.GoMiddleToStart = false; x.GoEndToStart = false; });
                SetHint("second", "first", x => { x.GoEndToMiddle = false; x.GoEndToStart = true; });
            }
        }
        #endregion

        [Scenario]
        public void Given()
        {
            wf = new Factory();
        }

        [Observation]
        [TestCase(null, "first")]
        [TestCase(null, "second")]
        [TestCase("first", "first")]
        [TestCase("first", "second")]
        [TestCase("second", "first")]
        [TestCase("second", "second")]
        [TestCase("second", null)]
        public void ObjectCanBeDirected(string from, string to)
        {
            var o = new WObject() { State = from };
            wf.TransitionTo(o, to);
            if (to != null)
                o.State.ShouldBe(to);
            else
                o.State.ShouldBeNull();
        }
    }
}

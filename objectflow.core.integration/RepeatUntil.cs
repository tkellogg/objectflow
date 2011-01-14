using System;
using NUnit.Framework;
using objectflow.core.tests.integration.TestOperations;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;

namespace objectflow.core.tests.integration.RepeatUntil
{
    public class WhenUsingRepeatUntil : Specification
    {
        IWorkflow<string> _workflow;
        private string _result;

        [Scenario]
        public void Given()
        {
            _result = string.Empty;
            _workflow = Workflow<string>.Definition()
                .Do((x) => x += "AB");
        }

        [Observation]
        public void SimpleRepeat()
        {
            _workflow.Repeat().Once();
            _result =_workflow.Start();

            _result.ShouldBe("ABAB");

        }

        [Observation]
        public void DefaultRepeat()
        {
            _workflow.Repeat();
            _result = _workflow.Start();

            _result.ShouldBe("ABAB");
        }

        [Observation]
        public void RepeatTwice()
        {
            _workflow.Repeat().Twice();
            _result = _workflow.Start();

            _result.ShouldBe("ABABAB");
        }

        [Observation]
        public void RepeatTimes()
        {
            _workflow.Repeat().Times(3);
            _result = _workflow.Start();

            _result.ShouldBe("ABABABAB");
        }

        [Observation, Ignore]
        public void RepeatUntilCondition()
        {
            // TODO: next release
            //_workflow.Repeat().Until(() => _result == "ABABAB");
            _result = _workflow.Start();

            _result.ShouldBe("ABABAB");
        }

        [Observation]
        public void RepeatWithDelay()
        {
            _workflow.Repeat().Twice().With.Interval.Of.Seconds(2);

            string beforeTime = DateTime.Now.ToLongTimeString();
            _workflow.Start();

            DateTime finishTime = DateTime.Now.Subtract(new TimeSpan(0, 0, 4));

            finishTime.ToLongTimeString().ShouldBe(beforeTime);
        }

        [Observation]
        public void ShouldRepeatOperation()
        {
            IWorkflow<Colour> workflow = new Workflow<Colour>();
            workflow.Do<DuplicateName>().Repeat().Twice();
            var result = workflow.Start(new Colour("Red"));

            result.Name.ShouldBe("RedRedRedRedRedRedRedRed");            
        }


    }
}
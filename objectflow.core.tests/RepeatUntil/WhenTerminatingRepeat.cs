using Objectflow.core.tests.TestOperations;
using Objectflow.tests.TestDomain;
using Rainbow.ObjectFlow.Framework;

namespace Objectflow.core.tests.RepeatUntil
{
    public class WhenTerminatingRepeat : Specification
    {
        private Workflow<Colour> _flow;

        [Scenario]
        public void Given()
        {
            _flow = new Workflow<Colour>();
        }

        [Observation]
        public void ShouldBeAbleToAddOperationAfterOnce()
        {
            _flow
                .Do<DoubleSpace>().Repeat().Once()
                .Then<Colour>()
                .Do<DoubleSpace>();

            _flow.RegisteredOperations.Tasks.Count.ShouldBe(2);
        }

        [Observation]
        public void ShouldAddOperationAfterTwice()
        {
            _flow
                .Do<DoubleSpace>().Repeat().Twice()
                .Then<Colour>()
                .Do<DoubleSpace>();

            _flow.RegisteredOperations.Tasks.Count.ShouldBe(2);
        }

        [Observation]
        public void ShouldAddOperationAfterTimes()
        {
            _flow
                .Do<DoubleSpace>().Repeat().Times(1)
                .Then<Colour>()
                .Do<DoubleSpace>();

            _flow.RegisteredOperations.Tasks.Count.ShouldBe(2);
        }

        [Observation]
        public void ShouldAddOperationAfterSeconds()
        {
            _flow
                .Do<DoubleSpace>().Repeat().Once().With.Interval.Of.Seconds(1)
                .Then<Colour>().Do<DoubleSpace>();

            _flow.RegisteredOperations.Tasks.Count.ShouldBe(2);
        }

        [Observation]
        public void ShouldAddOperationAfterMinutes()
        {
            _flow
                .Do<DoubleSpace>().Repeat().Once().With.Interval.Of.Minutes(1)
                .Then<Colour>()
                .Do<DoubleSpace>();

            _flow.RegisteredOperations.Tasks.Count.ShouldBe(2);
        }

        [Observation]
        public void ShouldAddOperationAfterMilliseconds()
        {
            _flow
                .Do<DoubleSpace>().Repeat().Once().With.Interval.Of.Milliseconds(1)
                .Then<Colour>()
                .Do<DoubleSpace>();

            _flow.RegisteredOperations.Tasks.Count.ShouldBe(2);
        }
    }
}

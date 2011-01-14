using NUnit.Framework;
using Rainbow.ObjectFlow.Policies;

namespace Objectflow.core.tests.RepeatUntil
{
    public class WhenUsingRepeatPolicy:Specification
    {
        private Repeat _repeat;

        [Scenario]
        public void Given_a_repeat_policy()
        {
            _repeat = new Repeat(null);
        }

        [Observation]
        public void Should_repeat_once()
        {
            _repeat.Count.ShouldBe(1);
        }

        [Observation]
        public void Should_repeat_twice()
        {
            _repeat.Twice();
            _repeat.Count.ShouldBe(2);
        }

        [Observation]
        public void Should_repeat_n_time()
        {
            _repeat.Times(4);
            _repeat.Count.ShouldBe(4);
        }
    }
}

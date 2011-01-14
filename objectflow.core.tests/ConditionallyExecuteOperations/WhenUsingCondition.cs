using System;
using Moq;
using NUnit.Framework;
using Rainbow.ObjectFlow.Constraints;
using Rainbow.ObjectFlow.Framework;

namespace Objectflow.core.tests
{
    public class WhenUsingCondition:Specification
    {
        [Observation]
        public void ShouldEvaluateTrue()
        {
            var condition = new Condition(new Func<bool>(() => true));
            Assert.That(condition.Matches(), Is.True);
        }

        [Observation]
        public void ShouldEvaluateFalse()
        {
            var condition = new Condition(new Func<bool>(() => false));
            Assert.That(condition.Matches(), Is.False);
        }

        [Observation]
        public void ShouldBeAbleToTypes()
        {
            var bo = new Mock<BasicOperation<string>>();
            var condition = new Condition<BasicOperation<string>>(new Func<BasicOperation<string>, bool>((b) => b.SuccessResult), bo.Object);
        }
    }
}

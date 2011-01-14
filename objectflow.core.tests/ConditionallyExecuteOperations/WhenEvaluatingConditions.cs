using NUnit.Framework;
using Rainbow.ObjectFlow.Constraints;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;

namespace Objectflow.tests
{
    public sealed class WhenEvaluatingConditions:Specification
    {
        private ICheckConstraint _checkBoolean;

        [Scenario]
        public void Given()
        {
        }

        [Observation]
        public void ShouldResolveFalseCheckCondition()
        {
            _checkBoolean = If.IsTrue(false);

            Assert.That(((Condition)_checkBoolean).Matches(), Is.False);
        }

        [Observation]
        public void ShouldResolveTrueCheckCondition()
        {
            _checkBoolean = If.IsTrue(true);

            Assert.That(((Condition)_checkBoolean).Matches(), Is.True);
        }
    }
}
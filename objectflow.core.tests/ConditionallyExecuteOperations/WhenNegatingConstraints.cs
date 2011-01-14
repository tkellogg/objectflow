using NUnit.Framework;
using Objectflow.core.tests.TestOperations;
using Rainbow.ObjectFlow.Helpers;

namespace Objectflow.tests
{
    public class WhenNegatingConstraints:Specification
    {
        [Observation]
        public void ShouldResolveNotOperationConstraint()
        {
            var operation = new DoubleSpace();
            var successConstraint = If.Not.Successfull(operation);

            Assert.That(successConstraint.Matches(), Is.True);
        }

        [Observation]
        public void ShouldPassIsFalseForFalseValue()
        {
            var successConstraint = If.IsFalse(false);

            Assert.That(successConstraint.Matches(), Is.True);
        }

        [Observation]
        public void ShouldNotPassNotIsTrueFalseForTrueValue()
        {
            var constraint = If.Not.IsTrue(true);

            Assert.That(constraint.Matches(), Is.False);
        }

        [Observation]
        public void ShouldNotPassIsFalseForTrueValue()
        {
            var constraint = If.IsFalse(true);

            Assert.That(constraint.Matches(), Is.False);
        }
    }
}
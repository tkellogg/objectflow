using NUnit.Framework;
using Objectflow.core.tests.TestOperations;
using Rainbow.ObjectFlow.Helpers;

namespace Objectflow.tests
{
    [TestFixture]
    public class WhenNegatingConstraints
    {
        [Test]
        public void ShouldResolveNotOperationConstraint()
        {
            var operation = new DoubleSpace();
            var successConstraint = If.Not.Successfull(operation);

            Assert.That(successConstraint.Matches(), Is.True);
        }

        [Test]
        public void ShouldPassIsFalseForFalseValue()
        {
            var successConstraint = If.IsFalse(false);

            Assert.That(successConstraint.Matches(), Is.True);
        }

        [Test]
        public void ShouldNotPassNotIsTrueFalseForTrueValue()
        {
            var constraint = If.Not.IsTrue(true);

            Assert.That(constraint.Matches(), Is.False);
        }

        [Test]
        public void ShouldNotPassIsFalseForTrueValue()
        {
            var constraint = If.IsFalse(true);

            Assert.That(constraint.Matches(), Is.False);
        }
    }
}
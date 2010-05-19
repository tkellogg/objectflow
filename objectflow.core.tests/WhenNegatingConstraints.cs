using NUnit.Framework;
using objectflow.tests.TestOperations;
using Rainbow.ObjectFlow.Helpers;

namespace objectflow.tests
{
    [TestFixture]
    public class WhenNegatingConstraints
    {
        [Test]
        public void ShouldResolveNotOperationConstraint()
        {
            var operation = new DoubleSpace();
            var successConstraint = When.Not.Successfull(operation);

            Assert.That(successConstraint.Matches(), Is.True);
        }

        [Test]
        public void ShouldPassIsFalseForFalseValue()
        {
            var successConstraint = When.IsFalse(false);

            Assert.That(successConstraint.Matches(), Is.True);
        }

        [Test]
        public void ShouldNotPassNotIsTrueFalseForTrueValue()
        {
            var constraint = When.Not.IsTrue(true);

            Assert.That(constraint.Matches(), Is.False);
        }

        [Test]
        public void ShouldNotPassIsFalseForTrueValue()
        {
            var constraint = When.IsFalse(true);

            Assert.That(constraint.Matches(), Is.False);
        }
    }
}
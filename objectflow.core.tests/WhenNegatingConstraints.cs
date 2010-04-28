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
    }
}
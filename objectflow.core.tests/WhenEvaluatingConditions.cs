using NUnit.Framework;
using Rainbow.ObjectFlow.Constraints;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;

namespace Objectflow.tests
{
    [TestFixture]
    public sealed class WhenEvaluatingConditions
    {
        private ICheckConstraint _checkBoolean;

        [SetUp]
        public void BeforeEachTest()
        {
        }

        [Test]
        public void ShouldResolveFalseCheckCondition()
        {
            _checkBoolean = If.IsTrue(false);

            Assert.That(((Condition)_checkBoolean).Matches(), Is.False);
        }

        [Test]
        public void ShouldResolveTrueCheckCondition()
        {
            _checkBoolean = If.IsTrue(true);

            Assert.That(((Condition)_checkBoolean).Matches(), Is.True);
        }
    }
}
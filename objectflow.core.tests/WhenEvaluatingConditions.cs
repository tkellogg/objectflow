using NUnit.Framework;
using Rainbow.ObjectFlow.Constraints;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;

namespace objectflow.tests
{
    [TestFixture]
    public sealed class WhenEvaluatingConditions
    {
        private ICheckContraint _checkBoolean;

        [SetUp]
        public void BeforeEachTest()
        {
        }

        [Test]
        public void ShouldResolveFalseCheckCondition()
        {
            _checkBoolean = When.IsTrue(false);

            Assert.That(((CheckConstraint)_checkBoolean).Matches(), Is.False);
        }

        [Test]
        public void ShouldResolveTrueCheckCondition()
        {
            _checkBoolean = When.IsTrue(true);

            Assert.That(((CheckConstraint)_checkBoolean).Matches(), Is.True);
        }
    }
}
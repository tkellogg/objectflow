using NUnit.Framework;
using Objectflow.tests.TestDomain;
using Objectflow.tests.TestOperations;
using Rainbow.ObjectFlow.Constraints;

namespace Objectflow.tests
{
    [TestFixture]
    public class WhenEvaluatingTypeConstraints
    {
        private CheckConstraint _successConstraint;
        private DuplicateName _duplicateName;
        private Colour _colour;

        [SetUp]
        public void BeforeEachTest()
        {
            _colour = new Colour("Red");
            _duplicateName = new DuplicateName();
        }

        [Test]
        public void ShouldResolvePositiveSuccess()
        {
            _successConstraint = new SuccessCheckConstraint<Colour>(_duplicateName);
            _duplicateName.Execute(_colour);

            Assert.That(_successConstraint.Matches(), Is.True);
        }

        [Test]
        public void ShouldResolveNegativeSuccess()
        {
            _successConstraint = new SuccessCheckConstraint<Colour>(_duplicateName);

            Assert.That(_successConstraint.Matches(), Is.False);
        }
    }
}
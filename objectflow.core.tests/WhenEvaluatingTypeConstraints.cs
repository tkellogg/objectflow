using NUnit.Framework;
using objectflow.tests.TestDomain;
using objectflow.tests.TestOperations;
using Rainbow.ObjectFlow.Constraints;

namespace objectflow.tests
{
    [TestFixture]
    public class WhenEvaluatingTypeConstraints
    {
        private CheckConstraint _successConstraint;
        private DuplicateName _duplicateName;
        private Colour[] _colours;

        [SetUp]
        public void BeforeEachTest()
        {
            _colours = new[] { new Colour("Red") };
            _duplicateName = new DuplicateName();
        }

        [Test]
        public void ShouldResolvePositiveSuccess()
        {
            _successConstraint = new SuccessCheckConstraint<Colour>(_duplicateName);
            _duplicateName.Execute(_colours);

            Assert.That(_successConstraint.Matches(), Is.True);
        }

        [Test]
        public void ShouldResolveNegativeSuccess()
        {
            _successConstraint = new SuccessCheckConstraint<Colour>(_duplicateName);

            Assert.That(_successConstraint.Matches(), Is.False);
        }

        // TODO: solve reference bug.
    }
}
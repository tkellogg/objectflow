using NUnit.Framework;
using Objectflow.tests.TestDomain;
using Objectflow.tests.TestOperations;
using Rainbow.ObjectFlow.Constraints;
using Rainbow.ObjectFlow.Interfaces;

namespace Objectflow.tests
{
    [TestFixture]
    public class WhenEvaluatingTypeConstraints
    {
        private ICheckConstraint _successConstraint;
        private DuplicateName _duplicateName;
        private Colour _colour;

        [SetUp]
        public void BeforeEachTest()
        {
            _colour = new Colour("Red");
            _duplicateName = new DuplicateName();
        }
    }
}
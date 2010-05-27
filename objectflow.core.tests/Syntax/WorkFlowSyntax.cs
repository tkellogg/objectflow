using NUnit.Framework;
using objectflow.tests.TestDomain;
using objectflow.tests.TestOperations;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;
using Rhino.Mocks;

namespace objectflow.tests.Syntax
{
    [TestFixture]
    public class WorkFlowSyntax
    {
        private Pipeline<Colour> _pipe;
        private IOperation<Colour> _doublespace = new DoubleSpace();
        private IOperation<Colour> _doublespaceOne = new DoubleSpace();
        private IOperation<Colour> _doubleSpaceTwo = new DoubleSpace();
        private Colour[] _redOnly;

        #region Setup and Teardown methods
        [TestFixtureSetUp]
        public void BeforeAnyTest()
        {
        }

        [TestFixtureTearDown]
        public void AfterAllTests()
        {
        }

        [SetUp]
        public void BeforeEachTest()
        {
            _redOnly = new[] { new Colour("Red") };
            _pipe = new Pipeline<Colour>();
            _pipe.Execute(new PipelineMemoryLoader<Colour>(_redOnly));
            _doublespace = new DoubleSpace();
            _doublespaceOne = new DoubleSpace();
            _doubleSpaceTwo = new DoubleSpace();
        }

        [TearDown]
        public void AfterEachTest()
        {
        }
        #endregion Setup and teardown methods

        [Test]
        public void SingleOperationSyntax()
        {
            _pipe.Execute(_doublespace);

            var result = WhenT();

            Assert.That(result, Is.Not.Null, "null");
            Assert.That(result.Name, Is.EqualTo("R e d"), "wrong value");
        }

        [Test]
        public void MultipleOperationSyntax()
        {
            _pipe.Execute(_doublespaceOne).Execute(_doubleSpaceTwo);

            var result = WhenT();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("R   e   d"));
        }

        [Test]
        public void SimpleBooleanConstraintSyntax()
        {
            _pipe.Execute(_doublespaceOne, When.IsTrue(false));

            var result = WhenT();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("Red"));
        }

        [Test]
        public void SimpleBooleanConstraintNegationSyntax()
        {
            _pipe.Execute(_doublespaceOne, When.Not.IsTrue(false));

            var result = WhenT();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("R e d"));
        }

        [Test]
        public void DecisionConstraintSyntax()
        {
            _pipe
                .Execute(_doublespaceOne)
                .Execute(_doubleSpaceTwo, When.Successful(_doublespaceOne));

            var result = WhenT();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("R   e   d"));
        }

        [Test]
        public void DecisionConstraintNegationSyntax()
        {
            var failedOperation = MockRepository.GenerateStub<BasicOperation<Colour>>();
            failedOperation.Stub<BasicOperation<Colour>>(
                bo => bo.Execute(null)).IgnoreArguments().Return(_redOnly);
            failedOperation.SetSuccessResult(false);

            _pipe
                .Execute(failedOperation)
                .Execute(_doubleSpaceTwo, When.Not.Successfull(_doublespaceOne));

            var result = WhenT();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("R e d"));

        }

        //[Test]
        //public void LamdaDecisionSyntax()
        //{
        //    string z = "red";

        //    _pipe
        //        .Execute(_doublespaceOne)
        //        .Execute(_doubleSpaceTwo, When.IsTrue(() => z.Contains("blue")));

        //    var result = WhenT();

        //    Assert.That(result, Is.Not.Null);
        //    Assert.That(result.ToString(), Is.EqualTo("R e d"));
        //}

        [Test]
        public void GenericPipelineSyntax()
        {
            var pipe = new Pipeline<Colour>();
            pipe.Execute(new PipelineMemoryLoader<Colour>(_redOnly))
                .Execute(_doublespace);

            var results = pipe.Start();

            var result = Pipeline<Colour>.GetItem(results, 0);

            Assert.That(result.ToString(), Is.EqualTo("R e d"));

        }

        private Colour WhenT()
        {
            var results = _pipe.Start();
            return Pipeline<Colour>.GetItem(results, 0);
        }
    }
}
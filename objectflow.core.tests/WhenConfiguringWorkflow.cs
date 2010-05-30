using System;
using NUnit.Framework;
using objectflow.tests.TestDomain;
using objectflow.tests.TestOperations;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;
using Rhino.Mocks;

namespace objectflow.tests
{
    [TestFixture]
    public class WhenConfiguringWorkflow
    {
        private IOperation<Colour> _doubleSpace;
        private IOperation<Colour> _duplicateName;
        private IOperation<Colour> _secondDuplicateName;
        private Pipeline<Colour> _workFlow;
        private MockRepository _mocker;
        private Colour _colour;
        private IOperation<Colour> _defaultLoader;

        [SetUp]
        public void BeforeEachTest()
        {
            _doubleSpace = new DoubleSpace();
            _duplicateName = new DuplicateName();
            _secondDuplicateName = new DuplicateName();
            _colour = new Colour("Red");
            _defaultLoader = new PipelineMemoryLoader<Colour>(_colour);

            _workFlow = new Pipeline<Colour>();
            _workFlow.Execute(_defaultLoader);

            _mocker = new MockRepository();
        }

        [Test]
        public void ShouldExecuteSingleOperation()
        {
            _workFlow.Execute(_doubleSpace);

            var result = _workFlow.Start();

            Assert.That(result, Is.Not.Null, "No results");
            Assert.That(result.ToString(), Is.EqualTo("R e d"), "Colour name value");
        }

        [Test]
        public void ShouldExecuteChainedOperations()
        {
            _workFlow.Execute(_duplicateName).Execute(_doubleSpace);

            var result = _workFlow.Start();

            Assert.That(result.ToString(), Is.EqualTo("R e d R e d"), "Colour name value");
        }

        [Test]
        public void ShouldNotExecuteFalseConditionalOperations()
        {
            _workFlow.Execute(_duplicateName).Execute(_doubleSpace, When.IsTrue(false));

            var result = _workFlow.Start();

            Assert.That(result.ToString(), Is.EqualTo("RedRed"));
        }

        [Test]
        public void ShouldExecuteTrueConditionalOperations()
        {
            _workFlow.Execute(_duplicateName).Execute(_doubleSpace, When.IsTrue(true));

            var result = _workFlow.Start();

            Assert.That(result.ToString(), Is.EqualTo("R e d R e d"));
        }

        [Test]
        public void ShouldChainNotOperationsCorrectly()
        {
            _workFlow
                .Execute(_duplicateName)
                .Execute(_doubleSpace, When.Not.Successfull(_duplicateName))
                .Execute(_secondDuplicateName, When.Successful(_duplicateName));

            var result = _workFlow.Start();

            Assert.That(result.ToString(), Is.EqualTo("RedRedRedRed"));
        }

        [Test]
        public void ShouldCheckForNullOperation()
        {
            BasicOperation<Colour> operation = null;
            Exception ex = Assert.Throws<ArgumentNullException>(() => _workFlow.Execute(operation), "thrown exception");
        }

        [Test]
        public void ShouldCheckForNullConstraint()
        {
            Exception exception = Assert.Throws<ArgumentNullException>(() => _workFlow.Execute(_duplicateName, null));

            Assert.That(exception.Message, Is.StringContaining("Argument [constraint] cannot be null"));
        }

        [Test]
        public void ShouldCheckForNullOperationWithConstraint()
        {
            IOperation<Colour> operation = null;
            Exception exception = Assert.Throws<ArgumentNullException>(() => _workFlow.Execute(operation, When.IsTrue(true)));

            Assert.That(exception.Message, Is.StringContaining("Argument [operation] cannot be null"));
        }

        [Test]
        public void ShouldCheckForNullOperationBeforeNullConstraint()
        {
            IOperation<Colour> operation = null;
            Exception exception = Assert.Throws<ArgumentNullException>(() => _workFlow.Execute(operation, null));

            Assert.That(exception.Message, Is.StringContaining("Argument [operation] cannot be null"));
        }

        [Test]
        public void ShouldRunOperationsInTopDownOrder()
        {

            var op1 = _mocker.PartialMock<DuplicateName>();
            var op2 = _mocker.PartialMock<DoubleSpace>();

            using (_mocker.Ordered())
            {
                Expect.Call(op1.Execute(_colour)).IgnoreArguments().Return(_colour);
                Expect.Call(op2.Execute(_colour)).IgnoreArguments().Return(_colour);
            }
            _mocker.ReplayAll();

            _workFlow
                .Execute(op1)
                .Execute(op2);

            var results = _workFlow.Start();

            _mocker.VerifyAll();

        }

        [Test]
        public void ShouldUseConstraintWithDeclaringOperationOnly()
        {
            var op1 = _mocker.Stub<DuplicateName>();
            var op2 = _mocker.Stub<DoubleSpace>();
            var op3 = _mocker.Stub<DuplicateName>();

            using (_mocker.Ordered())
            {
                Expect.Call(op1.Execute(_colour)).IgnoreArguments().Return(_colour);
                Expect.Call(op1.SetSuccessResult(true)).Return(true);

                Expect.Call(op2.Execute(_colour)).IgnoreArguments().Return(_colour);
                Expect.Call(op2.SetSuccessResult(true)).Return(false);

                Expect.Call(op3.Execute(_colour)).IgnoreArguments().Return(_colour);
                Expect.Call(op3.SetSuccessResult(true)).Return(true);
            }

            _workFlow
                .Execute(op1)
                .Execute(op2, When.Not.Successfull(op1))
                .Execute(op3, When.Successful(op1));

            _mocker.ReplayAll();

            var results = _workFlow.Start();

            _mocker.VerifyAll();
        }

        [Test]
        public void ShouldHandleOperationsInMultipleExecuteStatementsWithConstraintsAsDistinctInstances()
        {
            var op1 = _mocker.Stub<DuplicateName>();
            op1.SetSuccessResult(true);
            var op2 = _mocker.Stub<DoubleSpace>();

            using (_mocker.Ordered())
            {
                Expect.Call(op1.Execute(_colour)).IgnoreArguments().Return(_colour);
                Expect.Call(op1.SetSuccessResult(true)).Return(true);
                Expect.Call(op1.Execute(_colour)).IgnoreArguments().Return(_colour);
                Expect.Call(op1.SetSuccessResult(true)).Return(true);
            }

            _workFlow
                .Execute(op1)
                .Execute(op2, When.Not.Successfull(op1))
                .Execute(op1, When.Successful(op1));

            _mocker.ReplayAll();

            var results = _workFlow.Start();

            _mocker.VerifyAll();
        }

        [Test]
        public void ShouldBeAbleToAssignMultipleConstraintsToOperationExecutionClause()
        {
            _workFlow
                .Execute(_duplicateName)
                .Execute(_doubleSpace, When.Not.Successfull(_duplicateName))
                .Execute(_duplicateName, When.Successful(_duplicateName));

            var result = _workFlow.Start();

            Assert.That(result.ToString(), Is.EqualTo("RedRedRedRed"));
        }

        [Test]
        public void ShouldHandleMultipleTypeReuseWithConstraints()
        {
            _workFlow
                .Execute(_duplicateName)
                .Execute(_duplicateName, When.Not.Successfull(_duplicateName))
                .Execute(_duplicateName, When.Successful(_duplicateName))
                .Execute(_duplicateName, When.Successful(_duplicateName));

            var result = _workFlow.Start();

            Assert.That(result.ToString(), Is.EqualTo("RedRedRedRedRedRedRedRed"));

        }
    }
}
using System;
using NUnit.Framework;
using Objectflow.tests.TestDomain;
using Objectflow.tests.TestOperations;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;
using Rhino.Mocks;

namespace Objectflow.tests
{
    [TestFixture]
    public class WhenConfiguringWorkflow
    {
        private IOperation<Colour> _doubleSpace;
        private IOperation<Colour> _duplicateName;
        private IOperation<Colour> _secondDuplicateName;
        private Workflow<Colour> _workflow;
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
            _defaultLoader = new WorkflowMemoryLoader<Colour>(_colour);

            _workflow = new Workflow<Colour>();
            _workflow.Do(_defaultLoader);

            _mocker = new MockRepository();
        }

        [Test]
        public void ShouldExecuteSingleOperation()
        {
            _workflow.Do(_doubleSpace);

            var result = _workflow.Start();

            Assert.That(result, Is.Not.Null, "No results");
            Assert.That(result.ToString(), Is.EqualTo("R e d"), "Colour name value");
        }

        [Test]
        public void ShouldExecuteChainedOperations()
        {
            _workflow.Do(_duplicateName).Do(_doubleSpace);

            var result = _workflow.Start();

            Assert.That(result.ToString(), Is.EqualTo("R e d R e d"), "Colour name value");
        }

        [Test]
        public void ShouldNotExecuteFalseConditionalOperations()
        {
            _workflow.Do(_duplicateName).Do(_doubleSpace, If.IsTrue(false));

            var result = _workflow.Start();

            Assert.That(result.ToString(), Is.EqualTo("RedRed"));
        }

        [Test]
        public void ShouldExecuteTrueConditionalOperations()
        {
            _workflow.Do(_duplicateName).Do(_doubleSpace, If.IsTrue(true));

            var result = _workflow.Start();

            Assert.That(result.ToString(), Is.EqualTo("R e d R e d"));
        }

        [Test]
        public void ShouldChainNotOperationsCorrectly()
        {
            _workflow
                .Do(_duplicateName)
                .Do(_doubleSpace, If.Not.Successfull(_duplicateName))
                .Do(_secondDuplicateName, If.Successfull(_duplicateName));

            var result = _workflow.Start();

            Assert.That(result.ToString(), Is.EqualTo("RedRedRedRed"));
        }

        [Test]
        public void ShouldCheckForNullOperation()
        {
            BasicOperation<Colour> operation = null;
            Exception ex = Assert.Throws<ArgumentNullException>(() => _workflow.Do(operation), "thrown exception");
        }

        [Test]
        public void ShouldCheckForNullConstraint()
        {
            Exception exception = Assert.Throws<ArgumentNullException>(() => _workflow.Do(_duplicateName, null));

            Assert.That(exception.Message, Is.StringContaining("Argument [constraint] cannot be null"));
        }

        [Test]
        public void ShouldCheckForNullOperationWithConstraint()
        {
            IOperation<Colour> operation = null;
            Exception exception = Assert.Throws<ArgumentNullException>(() => _workflow.Do(operation, If.IsTrue(true)));

            Assert.That(exception.Message, Is.StringContaining("Argument [operation] cannot be null"));
        }

        [Test]
        public void ShouldCheckForNullOperationBeforeNullConstraint()
        {
            IOperation<Colour> operation = null;
            Exception exception = Assert.Throws<ArgumentNullException>(() => _workflow.Do(operation, null));

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

            _workflow
                .Do(op1)
                .Do(op2);

            var results = _workflow.Start();

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

            _workflow
                .Do(op1)
                .Do(op2, If.Not.Successfull(op1))
                .Do(op3, If.Successfull(op1));

            _mocker.ReplayAll();

            var results = _workflow.Start();

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

            _workflow
                .Do(op1)
                .Do(op2, If.Not.Successfull(op1))
                .Do(op1, If.Successfull(op1));

            _mocker.ReplayAll();

            var results = _workflow.Start();

            _mocker.VerifyAll();
        }

        [Test]
        public void ShouldBeAbleToAssignMultipleConstraintsToOperationExecutionClause()
        {
            _workflow
                .Do(_duplicateName)
                .Do(_doubleSpace, If.Not.Successfull(_duplicateName))
                .Do(_duplicateName, If.Successfull(_duplicateName));

            var result = _workflow.Start();

            Assert.That(result.ToString(), Is.EqualTo("RedRedRedRed"));
        }

        [Test]
        public void ShouldHandleMultipleTypeReuseWithConstraints()
        {
            _workflow
                .Do(_duplicateName)
                .Do(_duplicateName, If.Not.Successfull(_duplicateName))
                .Do(_duplicateName, If.Successfull(_duplicateName))
                .Do(_duplicateName, If.Successfull(_duplicateName));

            var result = _workflow.Start();

            Assert.That(result.ToString(), Is.EqualTo("RedRedRedRedRedRedRedRed"));
        }
    }
}
using System;
using System.Collections.Generic;
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
        private IEnumerable<Colour> _colours;
        private IOperation<Colour> _defaultLoader;

        [SetUp]
        public void BeforeEachTest()
        {
            _doubleSpace = new DoubleSpace();
            _duplicateName = new DuplicateName();
            _secondDuplicateName = new DuplicateName();
            _colours = new Colour[] { new Colour("Red") };
            _defaultLoader = new PipelineMemoryLoader<Colour>(_colours);

            _workFlow = new Pipeline<Colour>();
            _workFlow.Execute(_defaultLoader);

            _mocker = new MockRepository();
        }

        [Test]
        public void ShouldExecuteSingleOperation()
        {
            _workFlow.Execute(_doubleSpace);

            var results = _workFlow.Start();
            var result = Pipeline<Colour>.GetItem(results, 0);

            Assert.That(result, Is.Not.Null, "No results");
            Assert.That(result.ToString(), Is.EqualTo("R e d"), "Colour name value");
        }

        [Test]
        public void ShouldExecuteChainedOperations()
        {
            _workFlow.Execute(_duplicateName).Execute(_doubleSpace);

            var results = _workFlow.Start();
            var result = Pipeline<Colour>.GetItem(results, 0);

            Assert.That(result.ToString(), Is.EqualTo("R e d R e d"), "Colour name value");
        }

        [Test]
        public void ShouldNotExecuteFalseConditionalOperations()
        {
            _workFlow.Execute(_duplicateName).Execute(_doubleSpace, When.IsTrue(false));

            var results = _workFlow.Start();
            var result = Pipeline<Colour>.GetItem(results, 0);

            Assert.That(result.ToString(), Is.EqualTo("RedRed"));
        }

        [Test]
        public void ShouldExecuteTrueConditionalOperations()
        {
            _workFlow.Execute(_duplicateName).Execute(_doubleSpace, When.IsTrue(true));

            var results = _workFlow.Start();
            var result = Pipeline<Colour>.GetItem(results, 0);

            Assert.That(result.ToString(), Is.EqualTo("R e d R e d"));
        }

        [Test]
        public void ShouldChainNotOperationsCorrectly()
        {
            _workFlow
                .Execute(_duplicateName)
                .Execute(_doubleSpace, When.Not.Successfull(_duplicateName))
                .Execute(_secondDuplicateName, When.Successful(_duplicateName));

            var results = _workFlow.Start();
            var result = Pipeline<Colour>.GetItem(results, 0);

            Assert.That(result.ToString(), Is.EqualTo("RedRedRedRed"));
        }

        [Test]
        public void ShouldCheckForNullOperation()
        {
            Exception ex = Assert.Throws<ArgumentNullException>(() => _workFlow.Execute(null), "thrown exception");
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
            Exception exception = Assert.Throws<ArgumentNullException>(() => _workFlow.Execute(null, When.IsTrue(true)));

            Assert.That(exception.Message, Is.StringContaining("Argument [operation] cannot be null"));
        }

        [Test]
        public void ShouldCheckForNullOperationBeforeNullConstraint()
        {
            Exception exception = Assert.Throws<ArgumentNullException>(() => _workFlow.Execute(null, null));

            Assert.That(exception.Message, Is.StringContaining("Argument [operation] cannot be null"));
        }

        [Test]
        public void ShouldRunOperationsInTopDownOrder()
        {

            var op1 = _mocker.PartialMock<DuplicateName>();
            var op2 = _mocker.PartialMock<DoubleSpace>();

            using (_mocker.Ordered())
            {
                Expect.Call(op1.Execute(_colours)).IgnoreArguments().Return(_colours);
                Expect.Call(op2.Execute(_colours)).IgnoreArguments().Return(_colours);
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
            // TODO move these into new class or type constraint class.
            var op1 = _mocker.Stub<DuplicateName>();
            var op2 = _mocker.Stub<DoubleSpace>();
            var op3 = _mocker.Stub<DuplicateName>();

            using (_mocker.Ordered())
            {
                Expect.Call(op1.Execute(_colours)).IgnoreArguments().Return(_colours);
                Expect.Call(op1.SetSuccessResult(true)).Return(true);

                Expect.Call(op2.Execute(_colours)).IgnoreArguments().Return(_colours);
                Expect.Call(op2.SetSuccessResult(true)).Return(false);

                Expect.Call(op3.Execute(_colours)).IgnoreArguments().Return(_colours);
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
                Expect.Call(op1.Execute(_colours)).IgnoreArguments().Return(_colours);
                Expect.Call(op1.SetSuccessResult(true)).Return(true);
                Expect.Call(op1.Execute(_colours)).IgnoreArguments().Return(_colours);
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

            var results = _workFlow.Start();
            var result = Pipeline<Colour>.GetItem(results, 0);

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

            var results = _workFlow.Start();
            var result = Pipeline<Colour>.GetItem(results, 0);

            Assert.That(result.ToString(), Is.EqualTo("RedRedRedRedRedRedRedRed"));

        }

        [Test]
        public void ShouldReturnNullWhenPipelineNotConfiguredToLoadData()
        {
            var pipe = new Pipeline<Colour>();
            pipe.Execute(_duplicateName);

            var results = pipe.Start();
            var result = Pipeline<Colour>.GetItem(results, 0);

            Assert.That(result, Is.Null);

        }
    }
}
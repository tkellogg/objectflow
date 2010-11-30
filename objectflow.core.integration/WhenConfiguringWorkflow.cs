using System;
using NUnit.Framework;
using Objectflow.tests.TestDomain;
using Objectflow.tests.TestOperations;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;

namespace objectflow.core.integration
{
    [TestFixture]
    public class WhenConfiguringWorkflow
    {
        private IOperation<Colour> _doubleSpace;
        private IOperation<Colour> _duplicateName;
        private IOperation<Colour> _secondDuplicateName;
        private Workflow<Colour> _workflow;
        private Colour _colour;
        private IOperation<Colour> _defaultLoader;

        [SetUp]
        public void Given()
        {
            _doubleSpace = new DoubleSpace();
            _duplicateName = new DuplicateName();
            _secondDuplicateName = new DuplicateName();
            _colour = new Colour("Red");
            _defaultLoader = new WorkflowMemoryLoader<Colour>(_colour);

            _workflow = new Workflow<Colour>();
            _workflow.Do(_defaultLoader);
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

        [Test]
        public void ShouldAddOperationRegisteredByType()
        {
            string result =
                Workflow<Colour>.Definition()
                    .Do<DuplicateName>()
                    .Do<DuplicateName>(If.IsTrue(true))
                    .Start(new Colour("Red")).ToString();

            Assert.That(result, Is.EqualTo("RedRedRedRed"));
        }
    }
}
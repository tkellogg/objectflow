using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Objectflow.core.tests.TestOperations;
using Objectflow.tests.TestDomain;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Helpers;

namespace Objectflow.core.tests.ObjectCreation
{
    [TestFixture]
    public class WhenBuildingParallelOperationFromType
    {
        private ParallelSplitBuilder<Colour> _builder;

        [SetUp]
        public void Given()
        {
            _builder = new ParallelSplitBuilder<Colour>(new TaskList<Colour>());            
        }

        [Test]
        public void ShouldCreateInstance()
        {
            _builder.AddOperation<DuplicateName>();

            Assert.That(_builder.ParallelOperations.RegisteredOperations.Count, Is.EqualTo(1));
        }

        [Test]
        public void ShouldCreateInstanceAndConstraints()
        {
            _builder.AddOperation<DuplicateName>(If.IsTrue(true));

            Assert.That(_builder.ParallelOperations.RegisteredOperations.Count, Is.EqualTo(1));
        }
    }
}

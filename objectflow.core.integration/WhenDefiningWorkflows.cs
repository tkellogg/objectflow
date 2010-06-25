using System;
using Moq;
using NUnit.Framework;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;

namespace objectflow.core.integration
{
    [TestFixture]
    public class WhenDefiningWorkflows
    {

        [SetUp]
        public void Given()
        {

        }

        [Test]
        public void ShouldCheckOperationInstance()
        {
            var operation = new Mock<IOperation<string>>();
            Assert.Throws<InvalidCastException>(() => Workflow<string>.Definition().Do(operation.Object), "Exception");
        }

        [Test]
        public void ShouldCheckOperationInstanceWithConstraints()
        {
            var operation = new Mock<IOperation<string>>();
            Assert.Throws<InvalidCastException>(() => Workflow<string>.Definition().Do(operation.Object, If.IsTrue(true)), "Exception");
        }
    }
}

using System;
using Moq;
using NUnit.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;

namespace objectflow.core.integration
{
    [TestFixture]
    public class WhenCreatingConstraints
    {
        [SetUp]
        public void Given()
        {
        }

        [Test]
        public void ShouldCheckOperationInstanceOnSuccessfull()
        {
            var operation = new Mock<IOperation<string>>();

            Assert.Throws<InvalidCastException>(() => If.Successfull(operation.Object), "Exception");
        }

        [Test]
        public void ShouldCheckOperationInstanceOnNotSuccessful()
        {
            var operation = new Mock<IOperation<string>>();

            Assert.Throws<InvalidCastException>(() => If.Not.Successfull(operation.Object), "Exception");
        }


    }
}

using System;
using Moq;
using NUnit.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;

namespace Objectflow.core.tests.ConditionallyExecuteOperations
{
    public class WhenCreatingConstraints:Specification
    {
        [Scenario]
        public void Given()
        {          
        }

        [Observation]
        public void ShouldCheckOperationInstanceOnSuccessfull()
        {
            var operation = new Mock<IOperation<string>>();

            Assert.Throws<InvalidCastException>(() => If.Successfull(operation.Object), "Exception");
        }

        [Observation]
        public void ShouldCheckOperationInstanceOnNotSuccessful()
        {
            var operation = new Mock<IOperation<string>>();

            Assert.Throws<InvalidCastException>(() => If.Not.Successfull(operation.Object), "Exception");
        }


    }
}
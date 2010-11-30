using NUnit.Framework;
using Objectflow.core.tests.TestOperations;
using Objectflow.tests.TestDomain;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Helpers;

namespace Objectflow.core.tests
{
    [TestFixture]
    public class WhenComparingRegisteredOperations
    {
        private DoubleSpace _operation;

        [SetUp]
        public void Given()
        {
            _operation = new DoubleSpace();
            If.IsTrue(true);
        }

        [Test]
        public void ShouldReturnTrueForEqualOperations()
        {
            var operationDuplex = new OperationDuplex<Colour>(new OperationInvoker<Colour>(_operation));
            Assert.IsTrue(operationDuplex.Command.Equals(_operation));
        }

        [Test]
        public void ShouldReturnFalseForUnequalOperations()
        {
            var operationDuplex = new OperationDuplex<Colour>(new OperationInvoker<Colour>(_operation));
            Assert.IsFalse(operationDuplex.Command.Equals(new DoubleSpace()));
            
        }
    }
}
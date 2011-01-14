using NUnit.Framework;
using Objectflow.core.tests.TestOperations;
using Objectflow.tests.TestDomain;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;

namespace Objectflow.core.tests
{
    public class WhenComparingRegisteredOperations : Specification
    {
        private DoubleSpace _operation;

        [Scenario]
        public void Given()
        {
            _operation = new DoubleSpace();
        }

        [Observation]
        public void ShouldReturnTrueForEqualOperations()
        {
            var operationDuplex = new OperationDuplex<Colour>(new OperationInvoker<Colour>(_operation));
            operationDuplex.Command.ShouldEqual(_operation);
        }

        [Observation]
        public void ShouldReturnFalseForUnequalOperations()
        {
            var operationDuplex = new OperationDuplex<Colour>(new OperationInvoker<Colour>(_operation));
            operationDuplex.Command.ShouldNotEqual(new DoubleSpace());
        }
    }
}
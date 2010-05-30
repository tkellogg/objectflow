using System;
using NUnit.Framework;
using Rainbow.ObjectFlow.Constraints;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;

namespace objectflow.core.tests.Framework35
{
    [TestFixture]
    public class WhenDefiningFunctionalPipe
    {
        private Pipeline<string> _pipe;

        [SetUp]
        public void Given()
        {
            _pipe = new Pipeline<string>();
        }

        [Test]
        public void ShouldCheckIfMethodIsNull()
        {
            Func<string, string> method = null;
            Assert.Throws<ArgumentNullException>(() => _pipe.Execute(method), "Exception not thrown");
        }

        [Test]
        public void ShouldCheckIfConstraintIsNull()
        {
            CheckConstraint expression = null;
            _pipe = new Pipeline<string>();

            var method = new Func<string, string>((s) => { return "result"; });
            Assert.Throws<ArgumentNullException>(() => _pipe.Execute(method, expression), "Exception not thrown");

        }

        [Test]
        public void ShouldAddFunctionToRegisteredOperations()
        {
            _pipe.Execute(new Func<string, string>((b) => { return "true"; }));

            Assert.That(_pipe.RegisteredOperations.Count == 1);
        }

        [Test]
        public void ShouldAddConstraintToRegisteredOperations()
        {
            var constraint = When.IsTrue(true);
            _pipe.Execute(new Func<string, string>((b) => { return "true"; }), constraint);

            Assert.That(_pipe.RegisteredOperations.Count == 1, "Operation count");
            OperationConstraintPair<string> operation = _pipe.RegisteredOperations.Peek();

            Assert.That(operation.Constraint, Is.InstanceOf(typeof(BooleanCheckConstraint)), "Constraint type");
        }
    }
}

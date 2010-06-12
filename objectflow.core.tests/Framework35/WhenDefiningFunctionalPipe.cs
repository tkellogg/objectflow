using System;
using NUnit.Framework;
using Rainbow.ObjectFlow.Constraints;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;

namespace Objectflow.core.tests.Framework35
{
    [TestFixture]
    public class WhenDefiningFunctionalPipe
    {
        private Workflow<string> _pipe;

        [SetUp]
        public void Given()
        {
            _pipe = new Workflow<string>();
        }

        [Test]
        public void ShouldCheckIfMethodIsNull()
        {
            Func<string, string> method = null;
            Assert.Throws<ArgumentNullException>(() => _pipe.Do(method), "Exception not thrown");
        }

        [Test]
        public void ShouldCheckIfConstraintIsNull()
        {
            CheckConstraint expression = null;
            _pipe = new Workflow<string>();

            var method = new Func<string, string>((s) => { return "result"; });
            Assert.Throws<ArgumentNullException>(() => _pipe.Do(method, expression), "Exception not thrown");
        }

        [Test]
        public void ShouldAddFunctionToRegisteredOperations()
        {
            _pipe.Do(new Func<string, string>((b) => { return "true"; }));

            Assert.That(_pipe.RegisteredOperations.Count == 1);
        }

        [Test]
        public void ShouldAddConstraintToRegisteredOperations()
        {
            var constraint = If.IsTrue(true);
            _pipe.Do(new Func<string, string>((b) => { return "true"; }), constraint);

            Assert.That(_pipe.RegisteredOperations.Count == 1, "Operation count");
            OperationConstraintPair<string> operation = _pipe.RegisteredOperations[0];

            Assert.That(operation.Constraint, Is.InstanceOf(typeof(BooleanCheckConstraint)), "Constraint type");
        }
    }
}

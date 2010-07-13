using System;
using NUnit.Framework;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;

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
            ICheckConstraint expression = null;
            _pipe = new Workflow<string>();

            var method = new Func<string, string>((s) => { return "result"; });
            Assert.Throws<ArgumentNullException>(() => _pipe.Do(method, expression), "Exception not thrown");
        }

        [Test]
        public void ShouldAddFunctionToRegisteredOperations()
        {
            _pipe.Do(new Func<string, string>((b) => { return "true"; }));

            Assert.That(1 == _pipe.RegisteredOperations.Count);
        }

        [Test]
        public void ShouldAddConstraintToRegisteredOperations()
        {
            var constraint = If.IsTrue(true);
            _pipe.Do(new Func<string, string>((b) => "true"), constraint);

            Assert.That(1 ==_pipe.RegisteredOperations.Count, "Operation count");
            OperationConstraintPair<string> operation = _pipe.RegisteredOperations[0];
        }
    }
}

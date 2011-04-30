using System.Diagnostics;
using NUnit.Framework;
using Objectflow.core.tests.TestOperations;
using Objectflow.tests.TestDomain;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;

namespace Objectflow.core.tests
{
    public class WhenEvaluatingLamdaConstraints : Specification
    {
        private Workflow<Colour> _pipe;
        private IOperation<Colour> _duplicateNameOne;
        private IOperation<Colour> _duplicateNameTwo;
        private Colour _redOnly;
        private IOperation<Colour> _defaultLoader;

        [Scenario]
        public void Given()
        {
            _duplicateNameTwo = new DuplicateName();
            _duplicateNameOne = new DuplicateName();
            _duplicateNameTwo = new DuplicateName();
            _redOnly = new Colour("Red");
            _defaultLoader = new WorkflowMemoryLoader<Colour>(_redOnly);

            _pipe = new Workflow<Colour>();
            _pipe.Do(_defaultLoader);
        }

        [Observation]
        public void ShouldEvaluateNegativeExpression()
        {
            string z = "djnz";

            _pipe
                .Do(_duplicateNameOne)
                .Do(_duplicateNameTwo, If.IsTrue(() => z.Contains("nop")));

            var result = WhenRun();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("RedRed"));
        }

        [Observation]
        public void ShouldEvaluatePositiveExpression()
        {
            string z = "djnz";

            _pipe
                .Do(_duplicateNameOne)
                .Do(_duplicateNameTwo, If.IsTrue(() => z.Contains("dj")));

            var result = WhenRun();

            Assert.That(result.ToString(), Is.EqualTo("RedRedRedRed"));
        }

        private Colour WhenRun()
        {
            var result = _pipe.Start();
            return result;
        }
    }
}

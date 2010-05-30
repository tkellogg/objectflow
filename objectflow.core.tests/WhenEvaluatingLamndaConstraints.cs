using NUnit.Framework;
using objectflow.tests.TestDomain;
using objectflow.tests.TestOperations;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;

namespace objectflow.core.tests
{
    [TestFixture]
    public class WhenEvaluatingLamdaConstraints
    {
        private Pipeline<Colour> _pipe;
        private IOperation<Colour> _duplicateName;
        private IOperation<Colour> _duplicateNameOne;
        private IOperation<Colour> _duplicateNameTwo;
        private Colour _redOnly;
        private IOperation<Colour> _defaultLoader;

        [SetUp]
        public void BeforeEachTest()
        {
            _duplicateNameTwo = new DuplicateName();
            _duplicateNameOne = new DuplicateName();
            _duplicateNameTwo = new DuplicateName();
            _redOnly = new Colour("Red");
            _defaultLoader = new PipelineMemoryLoader<Colour>(_redOnly);

            _pipe = new Pipeline<Colour>();
            _pipe.Execute(_defaultLoader);
        }

        [Test]
        public void ShouldEvaluateNegativeExpression()
        {
            string z = "djnz";

            _pipe
                .Execute(_duplicateNameOne)
                .Execute(_duplicateNameTwo, When.IsTrue(() => z.Contains("nop")));

            var result = WhenRun();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("RedRed"));
        }

        [Test]
        public void ShouldEvaluatePositiveExpression()
        {
            string z = "djnz";

            _pipe
                .Execute(_duplicateNameOne)
                .Execute(_duplicateNameTwo, When.IsTrue(() => z.Contains("dj")));

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

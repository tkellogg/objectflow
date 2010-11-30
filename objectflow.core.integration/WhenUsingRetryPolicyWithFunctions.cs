using NUnit.Framework;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;

namespace objectflow.core.integration
{
    [TestFixture]
    public class WhenUsingRetryPolicyWithFunctions
    {
        private IWorkflow<string> _workflow;

        [SetUp]
        public void Given()
        {
            _workflow = Workflow<string>.Definition() as IWorkflow<string>;
        }

        [Test]
        public void ShouldNotThrowErrorWhenUsingRetryWithInterval()
        {
            _workflow.Do((s) => "Red").Retry().Twice().With.Interval.Of.Seconds(2);

            var result = _workflow.Start();

            Assert.That(result, Is.EqualTo("Red"));
        }


    }
}

using NUnit.Framework;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Language;
using Rhino.Mocks;

namespace Objectflow.core.tests.Syntax
{
    [TestFixture]
    public class RetryLoop
    {
        [Test]
        public void Retry()
        {
            Workflow<string>.Definition().Do(s => "Red").Retry();
        }
        
        [Test]
        public void RetrymanyWithIntervalOfMinutes()
        {
            Workflow<string>.Definition().Do(s => "Red").Retry().Attempts(4).With.Interval.Of.Minutes(5);
        }

        [Test]
        public void RetryOnce()
        {
            Workflow<string>.Definition().Do(s => "red").Retry().Once().With.Interval.Of.Milliseconds(500);
        }

        [Test]
        public void RetryTwiceWithInterval()
        {
            Workflow<string>.Definition().Do(s=>"Red").Retry().Twice().With.Interval.Of.Seconds(4);
        }
    }
}

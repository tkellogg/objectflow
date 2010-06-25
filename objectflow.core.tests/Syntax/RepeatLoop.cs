using System;
using NUnit.Framework;
using Rainbow.ObjectFlow.Framework;

//namespace Objectflow.core.tests.Syntax
//{
//    [TestFixture]
//    public class RepeatLoop
//    {
//        public void RepeatOnce()
//        {
//            Workflow<string>.Definition().Do((s) => "Red").Repeat().Once();
//        }

//        public void RepeatTwice()
//        {
//            Workflow<string>.Definition().Do(s => "red").Repeat().Twice();
//        }

//        public void RepeatMany()
//        {
//            Workflow<string>.Definition().Do(s => "red").Repeat().Times(Int32);
//            // Should include the with interval syntax on all the above.
//        }

//        public void RetryOnce()
//        {
//            Workflow<string>.Definition().Do((s) => "Red").Retry().Once().With.Interval.Of.Seconds(Int32);
//        }

//        public void RetryTwice()
//        {
//            Workflow<string>.Definition().Do(s => "red").Retry().Twice().With.Interval.of.Seconds(Int32);
//        }

//        public void RetryMany()
//        {
//            Workflow<string>.Definition().Do(s => "red").Retry().Times(Int32).With.Interval.of.Seconds(Int32);
//        }
//    }
//}

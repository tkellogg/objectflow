﻿using System;
using NUnit.Framework;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Language;

namespace Objectflow.core.tests.Policy
{
    [TestFixture]
    public class WhenUsingIntervalPolicy
    {
        private IInterval _interval;
        private Rainbow.ObjectFlow.Framework.Policy _policy;

        [SetUp]
        public void Given()
        {
            _interval = new Interval();
            _policy = _interval as Rainbow.ObjectFlow.Framework.Policy;
            Assert.IsNotNull(_policy);
        }

        [Test]
        public void ShouldWaitForSpecifiedTime()
        {
            _interval.Of.Seconds(1);
            
            string before = DateTime.Now.ToLongTimeString();

            _policy.Execute<string>("Red");
            
            Assert.That(DateTime.Now.Subtract(new TimeSpan(0, 0, 1)).ToLongTimeString(), Is.EqualTo(before), "TimeSpan incorrect");
        }
    }
}
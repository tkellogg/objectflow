using System;
using NUnit.Framework;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;

namespace Objectflow.core.tests
{
    [TestFixture]
    public class WhenDefiningWorkflows
    {
        [Test]
        public void ShouldCreateWorkflowInstance()
        {
            var wf = Workflow<string>.Definition();

            Assert.That(wf, Is.Not.Null);
            Assert.That(wf, Is.InstanceOf(typeof(IWorkflow<string>)));
        }

        [Test, Ignore]
        public void ShouldReturnUnsuccessfull()
        {
            var wf = Workflow<string>.Definition().Do(a =>
            {
                throw new ArgumentException("e");
            });

            wf.Start();
        }
    }
}

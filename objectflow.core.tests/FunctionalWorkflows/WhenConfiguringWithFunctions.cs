using System;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using NUnit.Framework;
using Rainbow.ObjectFlow.Container;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;

namespace Objectflow.core.tests.FunctionalWorkflows
{
    [TestFixture]
    public class WhenConfiguringWithFunctionsSpec
    {
        private IWorkflow<string> _workflow;
        private Func<bool> _function;
        private IKernel _container;
        private TaskList<string> _taskList;

        [SetUp]
        public void Given()
        {
            _taskList = new TaskList<string>();
            _container = new DefaultKernel();
            ServiceLocator<string>.SetInstance(_container);
            _container.Register(Component.For<TaskList<string>>().Instance(_taskList));
            _container.Register(Component.For<SequentialBuilder<string>>().Instance(new SequentialBuilder<string>(_taskList)));
            _container.Register(Component.For<WorkflowEngine<string>>().Instance(new WorkflowEngine<string>()));
            _workflow = new Workflow<string>();
            _function = new Func<bool>(RedOrangeYellow);
        }

        [Test]
        public void ShouldRegisterFunction()
        {
            When();

            Assert.That(_taskList.Tasks.Count == 1, "Number of registered operations");
        }

        [Test]
        public void ShouldRegisterFunctionWithConstraint()
        {
            var constraint = If.Not.Successfull(_function);
            _workflow.Do(_function, constraint);

            Assert.That(_taskList.Tasks.Count == 1, "Number of registered operations");
            Assert.That(_taskList.Tasks[0].Constraint == constraint, "Unexpected constraint object");                       
        }

        public void When()
        {
            _workflow.Do(_function);
            
        }

        private bool RedOrangeYellow()
        {
            _workflow.Context += "RichardOfYork";
            return true;
        }
    }
}
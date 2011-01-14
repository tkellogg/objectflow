using System;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using NUnit.Framework;
using Rainbow.ObjectFlow.Constraints;
using Rainbow.ObjectFlow.Container;
using Rainbow.ObjectFlow.Engine;
using Moq;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;

namespace Objectflow.core.tests.FunctionalWorkflows
{
    public class WhenExecutingOperations:Specification
    {
        private Dispatcher<string> _engine;
        private TaskList<string> _taskList;
        private DefaultKernel _container;
        private Func<string, string> _func;
        private Mock<FunctionInvoker<string>> _function;
        private Mock<ICheckConstraint> _constraint;

        [Scenario]
        public void Given()
        {
            _engine = new Dispatcher<string>();
            _taskList = new TaskList<string>();
            _container = new DefaultKernel();
            ServiceLocator<string>.SetInstance(_container);
            _container.Register(Component.For<TaskList<string>>().Instance(_taskList));

            _func = new Func<string, string>((s) => "Richard");
            _function = new Mock<FunctionInvoker<string>>(new object[] { _func });
            _constraint = new Mock<Condition>().As<ICheckConstraint>();
        }

        [Observation]
        public void ShouldExecuteOperation()
        {
            _function.Setup(s => s.Execute(null));

            _taskList.Tasks.Add(new OperationDuplex<string>(_function.Object));
            _engine.Execute(_taskList.Tasks);

            _function.VerifyAll();
        }

        [Observation]
        public void ShouldExecuteConstraints()
        {
            _function.Setup(s => s.Execute(null));
            _constraint.Setup(s => s.Matches()).Returns(true);
            _taskList.Tasks.Add(new OperationDuplex<string>(_function.Object, _constraint.Object));

            _engine.Execute(_taskList.Tasks);
            _function.VerifyAll();
            _constraint.VerifyAll();
        }
    }
}
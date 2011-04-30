using NUnit.Framework;
using Moq;
using Objectflow.core.tests.TestOperations;
using Objectflow.tests.TestDomain;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Language;

namespace Objectflow.core.tests.ObjectCreation
{
    public class WhenRegisteringTypeSpec:Specification
    {
        private Mock<SequentialBuilder<Colour>> _builder;
        private IDefine<Colour> _flow;

        [Scenario]
        public void Given()
        {
            _builder = new Mock<SequentialBuilder<Colour>>(new []{new TaskList<Colour>()});
			_flow = new Workflow<Colour>(new Dispatcher<Colour>(), _builder.Object, new ParallelSplitBuilder<Colour>(new TaskList<Colour>()));
        }

        [Observation]
        public void ShouldCreateInstanceOfSpecifiedOperation()
        {
            _builder.Setup(s => s.AddOperation<DuplicateName>());

            When();
            
            _builder.VerifyAll();
        }

        public void When()
        {            
            _flow.Do<DuplicateName>();
        }

        [Observation]
        public void ShouldCreateInstance()
        {
            var builder = new SequentialBuilder<Colour>(new TaskList<Colour>());
            builder.AddOperation<DuplicateName>();
            
            Assert.That(builder.TaskList.Tasks.Count, Is.EqualTo(1));
        }

        [Observation]
        public void ShouldCreateInstanceAndConstraints()
        {
            var builder = new SequentialBuilder<Colour>(new TaskList<Colour>());
            builder.AddOperation<DuplicateName>(If.IsTrue(true));

            Assert.That(builder.TaskList.Tasks.Count, Is.EqualTo(1));
        }
    }   
}

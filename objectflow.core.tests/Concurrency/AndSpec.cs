using NUnit.Framework;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;
using Rhino.Mocks;

namespace Objectflow.core.tests.Concurrency
{
    [TestFixture, Ignore]
    public class AndSpec
    {
        private MockRepository _mocker;
        private Workflow<string> _workflow;
        private IWorkflowBuilders<string> _factory;

        [SetUp]
        public void BeforeEachTest()
        {
            _mocker = new MockRepository();
            _factory = _mocker.StrictMock<IWorkflowBuilders<string>>();
            _workflow = new Workflow<string>(_factory);
        }

        [Test]
        public void ShouldReturnWorkflow()
        {
            var pipe = _workflow.And;

            Assert.That(pipe, Is.Not.Null, "object not instantiated");
            Assert.That(pipe.Equals(pipe), "workflow instance");
        }

        [Test]
        public void ShouldCreateParallelBuilder()
        {
            Expect.Call(_factory.GetParallelBuilder(_workflow)).Return(new ParallelSplitBuilder<string>(_workflow));
            _mocker.ReplayAll();
            var pipe = _workflow.And;

            _factory.VerifyAllExpectations();
        }

        [Test]
        public void ShouldNotCreateParallelBuilderOnSubsequentCalls()
        {
            Expect.Call(_factory.GetParallelBuilder(_workflow))
                .Return(new ParallelSplitBuilder<string>(_workflow))
                .Repeat.Once();

            _mocker.ReplayAll();
            var pipe = _workflow.And.And;

            _factory.VerifyAllExpectations();
        }
    }
}

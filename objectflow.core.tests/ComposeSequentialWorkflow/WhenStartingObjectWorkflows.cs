using NUnit.Framework;
using Objectflow.core.tests.TestOperations;
using Objectflow.tests.TestDomain;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rhino.Mocks;

namespace Objectflow.core.tests.ComposeSequentialWorkflow
{
    public class WhenStartingObjectWorkflows : Specification
    {
        private MockRepository _mocker;
        private Colour _colour;
        private Workflow<Colour> _workflow;

        [Scenario]
        public void Given()
        {
            _mocker = new MockRepository();
            _workflow = Workflow<Colour>.Definition() as Workflow<Colour>;
        }

        [Observation]
        public void ShouldRunOperationsInTopDownOrder()
        {
            var op1 = _mocker.PartialMock<DuplicateName>();
            var op2 = _mocker.PartialMock<DoubleSpace>();

            using (_mocker.Ordered())
            {
                Expect.Call(op1.Execute(_colour)).IgnoreArguments().Return(_colour);
                Expect.Call(op2.Execute(_colour)).IgnoreArguments().Return(_colour);
            }

            _mocker.ReplayAll();

            _workflow
                .Do(op1)
                .Do(op2);

            var results = _workflow.Start();

            _mocker.VerifyAll();
        }

        [Observation]
        public void ShouldUseConstraintWithDeclaringOperationOnly()
        {
            var op1 = _mocker.Stub<DuplicateName>();
            var op2 = _mocker.Stub<DoubleSpace>();
            var op3 = _mocker.Stub<DuplicateName>();

            using (_mocker.Ordered())
            {
                Expect.Call(op1.Execute(_colour)).IgnoreArguments().Return(_colour);
                Expect.Call(op1.SetSuccessResult(true)).Return(true);

                Expect.Call(op2.Execute(_colour)).IgnoreArguments().Return(_colour);
                Expect.Call(op2.SetSuccessResult(true)).Return(false);

                Expect.Call(op3.Execute(_colour)).IgnoreArguments().Return(_colour);
                Expect.Call(op3.SetSuccessResult(true)).Return(true);
            }

            _workflow
                .Do(op1)
                .Do(op2, If.Not.Successfull(op1))
                .Do(op3, If.Successfull(op1));

            _mocker.ReplayAll();

            var results = _workflow.Start();

            _mocker.VerifyAll();
        }

        [Observation]
        public void ShouldHandleOperationsInMultipleExecuteStatementsWithConstraintsAsDistinctInstances()
        {
            var op1 = _mocker.Stub<DuplicateName>();
            op1.SetSuccessResult(true);
            var op2 = _mocker.Stub<DoubleSpace>();

            using (_mocker.Ordered())
            {
                Expect.Call(op1.Execute(_colour)).IgnoreArguments().Return(_colour);
                Expect.Call(op1.SetSuccessResult(true)).Return(true);
                Expect.Call(op1.Execute(_colour)).IgnoreArguments().Return(_colour);
                Expect.Call(op1.SetSuccessResult(true)).Return(true);
            }

            _workflow
                .Do(op1)
                .Do(op2, If.Not.Successfull(op1))
                .Do(op1, If.Successfull(op1));

            _mocker.ReplayAll();

            var results = _workflow.Start();

            _mocker.VerifyAll();
        }

    }
}
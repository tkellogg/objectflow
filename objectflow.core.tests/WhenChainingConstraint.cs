using NUnit.Framework;
using Rainbow.ObjectFlow.ConstraintOperators;
using Rainbow.ObjectFlow.Constraints;
using Rhino.Mocks;

namespace objectflow.core.tests
{
    [TestFixture]
    public class WhenChainingConstraint
    {
        private MockRepository _mocker;

        [SetUp]
        public void BeforeEachTest()
        {
            _mocker = new MockRepository();
        }

        [Test]
        public void ShouldSetNext()
        {
            CheckConstraint constraint1 = new NotConstraintOperator();
            CheckConstraint constraint2 = new BooleanCheckConstraint(true);
            constraint1.SetNext(constraint2);

            Assert.That(constraint1.Next == constraint2);

        }

        [Test]
        public void ShouldEvaluateSingleChain()
        {
            CheckConstraint constraint1 = new BooleanCheckConstraint(false);
            Assert.That(constraint1.Matches(), Is.EqualTo(false));
        }

        [Test]
        public void ShouldEvaluateChainedBooleanExpressions()
        {
            CheckConstraint constraint1 = new NotConstraintOperator();
            CheckConstraint constraint2 = _mocker.PartialMock<BooleanCheckConstraint>(new object[] { true });
            Expect.Call(constraint2.Matches(true)).Return(true);
            _mocker.ReplayAll();

            constraint1.SetNext(constraint2);

            Assert.That(constraint2.Matches(), Is.EqualTo(true), "Second constraint");
            Assert.That(constraint1.Matches(true), Is.EqualTo(false), "First constraint");
            _mocker.VerifyAll();
        }

    }
}

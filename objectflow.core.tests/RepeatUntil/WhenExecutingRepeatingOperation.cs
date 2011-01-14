using Moq;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Policies;

namespace Objectflow.core.tests.RepeatUntil
{
    public class WhenExecutingRepeatingOperation : Specification
    {
        private Mock<FunctionInvoker<string>> _method;
        private Repeat _repeat;

        [Scenario]
        public void Given()
        {
            _method = new Mock<FunctionInvoker<string>>();
            _method.Setup(s => s.Execute("Red")).Returns("Red");
            _method.Object.Policies.Add(new Repeat(null) { Count = 3 });
        }

        [Observation]
        public void Should_exexute_multiple_times()
        {            
            _repeat = new Repeat(null){Count=3};
            _repeat.SetInvoker(_method.Object);

            _repeat.Execute("");

            _method.Verify(s => s.Execute(It.IsAny<string>()), Times.Exactly(3));
        }
    }
}

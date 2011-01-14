using Rainbow.ObjectFlow.Framework;

namespace objectflow.core.tests.integration.TestOperations
{
    public class OperationThatFailsNTimes : BasicOperation<string>
    {
        public int ExecuteCount;
        private readonly int _failcount;

        public OperationThatFailsNTimes()
            : this(1)
        {

        }

        public OperationThatFailsNTimes(int failcount)
        {
            _failcount = failcount;
        }

        public override string Execute(string data)
        {
            ExecuteCount++;

            if (ExecuteCount <= _failcount)
                SetSuccessResult(false);
            else
                SetSuccessResult(true);

            return data;
        }
    }
}
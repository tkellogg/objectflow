using Rainbow.ObjectFlow.Framework;

namespace Objectflow.core.tests.Policy
{
    public class FailOnceOperation : BasicOperation<string>
    {
        private static bool _fail = true;
        public int ExecuteCount;

        public override string Execute(string data)
        {
            ExecuteCount++;

            SetSuccessResult(!_fail);
            _fail = !_fail;

            return data;
        }
    }
}
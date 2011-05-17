using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Stateful
{
    internal class Transition : ITransition
    {
        public Transition(object workflowId, object from, object to)
        {
            WorkflowId = workflowId;
            From = from;
            To = to;
        }

        public object WorkflowId { get; private set; }
        public object From { get; private set; }
        public object To { get; private set; }

        public override bool Equals(object obj)
        {
			if (obj is ITransition && obj != null)
			{
				var o = (ITransition)obj;
				return object.Equals(From, o.From) && object.Equals(To, o.To)
					&& object.Equals(WorkflowId, o.WorkflowId);
			}
			else return false;
        }

        public override int GetHashCode()
        {
			unchecked
			{
				int multiplier = 31;
				int ret = 0;
				if (WorkflowId != null)
					ret = (ret + WorkflowId.GetHashCode()) * multiplier;
				if (From != null)
					ret = (ret + From.GetHashCode()) * multiplier;
				if (To != null)
					ret = (ret + To.GetHashCode()) * multiplier;
				return ret;
			}
        }

		public override string ToString()
		{
			return string.Format("Transition<From: {0}, To: {1}, Workflow: {2}>", From,
				To, WorkflowId);
		}
    }
}

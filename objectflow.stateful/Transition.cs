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
            if (obj is ITransition)
            {
                var o = (ITransition)obj;
                return From == o.From && To == o.To;
            }
            else return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            string code = string.Format("Rainbow.ObjectFlow.Stateful.Transition<<{0}|{1}>>", From, To);
            return code.GetHashCode();
        }
    }
}

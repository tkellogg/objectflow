using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ObjectFlow.Stateful
{
    internal class Transition : ITransition
    {
        public Transition(object from, object to)
        {
            From = from;
            To = to;
        }

        public object From { get; set; }

        public object To { get; set; }

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

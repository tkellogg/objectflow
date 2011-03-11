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
    }
}

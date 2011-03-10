using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Framework;

namespace Objectflow.core.tests.DeclaredOperations
{
    public class WithRedefinitions : Specification
    {
        [Observation]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RedefinitionThrowsException()
        {
            var op = Declare.Step();
            new Workflow<string>()
            .Do(x => x, op)
            .Do(x => x, op);
        }
    }
}

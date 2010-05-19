using System.Collections.Generic;
using objectflow.tests.TestDomain;
using Rainbow.ObjectFlow.Framework;
using System;

namespace objectflow.tests.TestOperations
{
    public class DoubleSpace : BasicOperation<Colour>
    {
        //        public event EventHandler Finished;
        //public class FinishedEventArgs : EventArgs
        //{
        //    public FinishedEventArgs(bool succeeded)
        //    {
        //        Success = succeeded;
        //    }

        //    public bool Success { get; set; }
        //}

        public override IEnumerable<Colour> Execute(IEnumerable<Colour> input)
        {
            foreach (Colour member in input)
            {
                string name = string.Empty;
                char[] chars = member.Name.ToCharArray();
                foreach (var c in chars)
                {
                    name = name + c + " ";
                }

                member.Name = name.Trim();
            }

            SetSuccessResult(GetSuccessResult());

            return input;
        }

        public virtual bool GetSuccessResult()
        {
            return true;
        }
    }
}
using System.Collections.Generic;
using Rainbow.ObjectFlow.Constraints;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.ConstraintOperators
{
    /// <summary>
    /// Defines constraints allowed with ConstraintOperators.
    /// </summary>
    public abstract class ConstraintOperator : CheckConstraint
    {
        private static Stack<CheckConstraint> ConstraintStack
        {
            get
            {
                return When.ConstraintStack;
            }
        }

        public SuccessCheckConstraint<T> Successfull<T>(IOperation<T> operaton)
        {
            IList<CheckConstraint> exp = UnwindStack();

            return new SuccessCheckConstraint<T>(operaton, exp);
        }

        public BooleanCheckConstraint IsTrue(bool condition)
        {
            IList<CheckConstraint> exp = UnwindStack();

            return new BooleanCheckConstraint(exp);
        }

        private static IList<CheckConstraint> UnwindStack()
        {
            IList<CheckConstraint> exp = new List<CheckConstraint>();
            while (ConstraintStack.Count > 0)
            {
                exp.Add(ConstraintStack.Pop());
            }

            return exp;
        }
    }
}
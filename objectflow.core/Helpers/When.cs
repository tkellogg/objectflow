using System.Collections.Generic;
using Rainbow.ObjectFlow.ConstraintOperators;
using Rainbow.ObjectFlow.Constraints;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Helpers
{
    /// <summary>
    /// Helper class for constraints
    /// </summary>
    public sealed class When
    {
        private static Stack<CheckConstraint> _constraintStack;

        static When()
        {
            _constraintStack = new Stack<CheckConstraint>();
        }

        internal static Stack<CheckConstraint> ConstraintStack
        {
            get
            {
                return _constraintStack;
            }
            set
            {
                _constraintStack = value;
            }
        }


        public static BooleanCheckConstraint IsTrue(bool condition)
        {
            return new BooleanCheckConstraint(condition);
        }

        public static SuccessCheckConstraint<T> Successful<T>(IOperation<T> operation)
        {
            return new SuccessCheckConstraint<T>(operation);
        }

        public static ConstraintOperator Not
        {
            get
            {
                ConstraintStack.Clear();
                var not = new NotConstraintOperator();
                ConstraintStack.Push(not);
                return not;
            }
        }
    }
}
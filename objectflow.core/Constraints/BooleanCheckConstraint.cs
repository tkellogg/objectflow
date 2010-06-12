using System;
using System.Collections.Generic;

namespace Rainbow.ObjectFlow.Constraints
{
    /// <summary>
    /// Constraint that evalueates boolean conditions
    /// </summary>
    public sealed class BooleanCheckConstraint : CheckConstraint
    {
        private bool _operation;
        private Func<bool> _funcConstraint = null;

        internal BooleanCheckConstraint(bool operation)
        {
            _operation = operation;
        }

        internal BooleanCheckConstraint(IList<CheckConstraint> constraints, bool operation)
        {
            _constraints = constraints;
            _operation = operation;
        }

        internal BooleanCheckConstraint(Func<bool> constraint)
        {
            _funcConstraint = constraint;
        }

        internal override bool Matches()
        {
            if (HasConstraints())
            {
                return MatchTree(_operation);
            }

            // TODO what if it has both? is this possible in current framework?
            if (HasLamdaConstraints())
            {
                return _funcConstraint();
            }

            return _operation;
        }

        private bool HasLamdaConstraints()
        {
            return _funcConstraint != null;
        }

        /// <summary>
        /// Evaluates the expression
        /// </summary>
        /// <param name="condition">condition to match the expression to</param>
        /// <returns>True if the expression evaluates to the condition parameter, false otherwise</returns>
        public override bool Matches(bool condition)
        {
            if (HasConstraints())
            {
                return MatchTree(condition);
            }

            return _operation == condition;
        }
    }
}
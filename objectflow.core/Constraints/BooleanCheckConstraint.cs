
using System.Collections.Generic;

namespace Rainbow.ObjectFlow.Constraints
{
    public sealed class BooleanCheckConstraint : CheckConstraint
    {
        private bool _operation;
        private IList<CheckConstraint> _constraints;

        internal BooleanCheckConstraint(bool operation)
        {
            _operation = operation;
        }

        internal BooleanCheckConstraint(IList<CheckConstraint> constraints)
        {
            _constraints = constraints;
        }

        internal override bool Matches()
        {

            if (_constraints == null || _constraints.Count == 0)
            {
                return _operation;
            }

            return MatchTree(_operation);
        }

        public override bool Matches(bool condition)
        {
            if (_constraints == null || _constraints.Count == 0)
            {
                return _operation == condition;
            }

            return MatchTree(condition);
        }

        private bool MatchTree(bool seed)
        {
            if (_constraints == null || _constraints.Count == 0)
                return true;

            bool match = seed;
            foreach (var constraint in _constraints)
            {
                match = constraint.Matches(match);
            }

            return match;
        }
    }
}
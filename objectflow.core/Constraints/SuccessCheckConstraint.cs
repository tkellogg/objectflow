using System.Collections.Generic;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Constraints
{
    public sealed class SuccessCheckConstraint<T> : CheckConstraint
    {
        private BasicOperation<T> _operation;
        private IList<CheckConstraint> _constraints;

        internal SuccessCheckConstraint(IOperation<T> operation)
        {
            _constraints = new List<CheckConstraint>();
            _operation = operation as BasicOperation<T>;
        }

        internal SuccessCheckConstraint(IOperation<T> operation, IList<CheckConstraint> constraints)
            : this(operation)
        {
            _constraints = constraints;
        }

        internal override bool Matches()
        {

            if (_constraints == null || _constraints.Count == 0)
            {
                return _operation.SuccessResult;
            }

            return MatchTree(_operation.SuccessResult);
        }

        public bool Matches(BasicOperation<T> condition)
        {

            if (_constraints == null || _constraints.Count == 0)
            {
                return condition.SuccessResult == _operation.SuccessResult;
            }

            return MatchTree(condition.SuccessResult);
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
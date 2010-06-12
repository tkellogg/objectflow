using System.Collections.Generic;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Constraints
{
    /// <summary>
    /// Constraint that evaluates an operations success or failure
    /// </summary>
    /// <typeparam name="T">Type of object the class will expect</typeparam>
    public sealed class SuccessCheckConstraint<T> : CheckConstraint
    {
        private BasicOperation<T> _operation;

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
            if (HasConstraints())
            {
                return MatchTree(_operation.SuccessResult);
            }

            return _operation.SuccessResult;
        }
    }
}
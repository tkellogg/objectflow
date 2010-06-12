using System.Collections.Generic;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Constraints
{
    /// <summary>
    /// Base Constaint class that specialised constraints inherit from. 
    /// </summary>
    public abstract class CheckConstraint : ICheckContraint
    {
        /// <summary>
        /// holds the constraints that apply to this.
        /// </summary>
        protected IList<CheckConstraint> _constraints;

        internal virtual bool Matches()
        {
            return false;
        }

        /// <summary>
        /// Evaluates whether the CheckConstraint maches a boolean value
        /// </summary>
        /// <param name="matches">boolean value to match to</param>
        /// <returns>true if the condition evalues to the matches parameter.</returns>
        public virtual bool Matches(bool matches)
        {
            return false;
        }

        /// <summary>
        /// Tests all constraints in the object graph
        /// </summary>
        /// <param name="matchWith"></param>
        /// <returns>True if all matches evaluated to true, false otherwise</returns>
        protected virtual bool MatchTree(bool matchWith)
        {
            if (!HasConstraints())
            {
                return true;
            }

            bool match = matchWith;
            foreach (var constraint in _constraints)
            {
                match = constraint.Matches(match);
            }

            return match;
        }

        /// <summary>
        /// Indicates whether constraints have been associated with this instance.
        /// </summary>
        /// <returns>True if constraints exist, false otherwise</returns>
        protected bool HasConstraints()
        {
            return !(null == _constraints || 0 == _constraints.Count);
        }
    }
}
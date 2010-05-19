using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Constraints
{
    /// <summary>
    /// Base Constaint class that specialised constraints inherit from. 
    /// </summary>
    public abstract class CheckConstraint : ICheckContraint
    {
        internal virtual bool Matches()
        {
            return false;
        }

        /// <summary>
        /// evaluates the condition
        /// </summary>
        /// <param name="condition">condition to evaluate</param>
        /// <returns>Result as evaluation</returns>
        public virtual bool Matches(BasicOperation<CheckConstraint> condition)
        {
            return true;
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
    }
}
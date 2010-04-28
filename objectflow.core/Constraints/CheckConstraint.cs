using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Constraints
{
    public abstract class CheckConstraint : ICheckContraint
    {
        internal virtual bool Matches()
        {
            return false;
        }

        public virtual bool Matches(BasicOperation<CheckConstraint> condition)
        {
            return true;
        }

        public virtual bool Matches(bool matches)
        {
            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Constraints
{
    /// <summary>
    /// Container class for constraint functions built by helper class
    /// </summary>
    public class Condition : ICheckConstraint
    {
        /// <summary></summary>
        protected internal readonly Func<bool> condition;
        /// <summary></summary>
        protected internal readonly IList<ICheckConstraint> contraints;


        ///<summary>
        /// Provided for mocking framework
        ///</summary>
        internal Condition()
        {

        }

        ///<summary>
        ///</summary>
        ///<param name="condition"></param>
        public Condition(Func<bool> condition)
        {
            this.condition = condition;
        }

        ///<summary>
        ///</summary>
        ///<param name="condition"></param>
        ///<param name="constraints"></param>
        ///<exception cref="NotImplementedException"></exception>
        public Condition(Func<bool> condition, IList<ICheckConstraint> constraints)
            : this(condition)
        {
            this.contraints = constraints;
        }

        /// <summary>
        /// Evaluates the constraint
        /// </summary>
        /// <returns>True if the constraint evaluated to true, false otherwise</returns>
        public virtual bool Matches()
        {
            if (contraints != null && contraints.Count > 0)
            {
                bool match = Matches(condition.Invoke());
                return match;
            }

            return condition.Invoke();
        }

        /// <summary>
        /// Evaluates the constraint
        /// </summary>
        /// <returns>True if the constraint evaluated to true, false otherwise</returns>
        public virtual bool Matches(bool matches)
        {
            bool match = false;
            for (int i = 0; i < contraints.Count; i++)
            {
                match = contraints[i].Matches(matches);
            }

            return match;
        }
    }
}

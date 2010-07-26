using System;
using System.Collections.Generic;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Constraints
{
    /// <summary>
    /// Container class for constraint functions built by helper class
    /// </summary>
    public class Condition<T> : ICheckConstraint
    {
        private readonly Func<T, bool> _condition;
        private T _operation;

        ///<summary>
        /// Default constructor
        ///</summary>
        public Condition(Func<T, bool> condition, T operation)
        {
            _condition = condition;
            _operation = operation;
        }
        
        /// <summary>
        /// Evaluates the constraint
        /// </summary>
        /// <returns>True if the constraint evaluated to true, false otherwise</returns>
        public bool Matches()
        {
            return bool.Parse(_condition.Invoke(_operation).ToString());
        }

        /// <summary>
        /// Evaluates the constraint
        /// </summary>
        /// <returns>True if the constraint evaluated to true, false otherwise</returns>
        public bool Matches(bool match)
        {
            return false;
        }
    }
}

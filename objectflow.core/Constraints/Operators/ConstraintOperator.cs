using System.Collections.Generic;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Constraints.Operators
{
    /// <summary>
    /// Defines constraints allowed with ConstraintOperators.
    /// </summary>
    public abstract class ConstraintOperator : ICheckConstraint
    {
        private static Stack<ICheckConstraint> ExecutionPlan
        {
            get
            {
                return Dispatcher<ICheckConstraint>.ExecutionPlan;
            }
        }

        /// <summary>
        /// Creates a SuccessCheckConstraint that can work with BasicOperations of T
        /// </summary>
        /// <typeparam name="T">Type the Constraint works with on</typeparam>
        /// <param name="operaton">the operation the constraint evaluates as successding or failing</param>
        /// <returns>A SuccessCheckConstraint</returns>
        public ICheckConstraint Successfull<T>(IOperation<T> operaton)
        {
            Check.IsInstanceOf<BasicOperation<T>>(operaton, "operation");

            IList<ICheckConstraint> exp = UnwindStack();
            var bo = operaton as BasicOperation<T>;
            return new Condition(() => bo.SuccessResult, exp);
        }

        /// <summary>
        /// Creates a BooleanCheckConstraint that works with Booleans.
        /// </summary>
        /// <param name="condition">true/False</param>
        /// <returns>A BooleanCheckConstraint that can evaluate a boolean value</returns>
        public ICheckConstraint IsTrue(bool condition)
        {
            IList<ICheckConstraint> exp = UnwindStack();

            return new Condition(() => condition, exp);
        }

        private static IList<ICheckConstraint> UnwindStack()
        {
            IList<ICheckConstraint> exp = new List<ICheckConstraint>();
            while (ExecutionPlan.Count > 0)
            {
                exp.Add(ExecutionPlan.Pop());
            }

            return exp;
        }

        #region ICheckConstraint Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract bool Matches();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matches"></param>
        /// <returns></returns>
        public abstract bool Matches(bool matches);
        #endregion

        /// <summary>
        /// Successfulls the specified _function.
        /// </summary>
        /// <param name="function">The _function.</param>
        /// <returns></returns>
        public ICheckConstraint Successfull(System.Func<bool> function)
        {
            return new Condition(()=>false);
        }
    }
}
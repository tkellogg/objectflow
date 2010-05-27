using System;
using System.Collections.Generic;
using Rainbow.ObjectFlow.ConstraintOperators;
using Rainbow.ObjectFlow.Constraints;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Helpers
{
    /// <summary>
    /// Helper class for constraints
    /// </summary>
    public sealed class When
    {
        static When()
        {
            ConstraintStack = new Stack<CheckConstraint>();
        }

        internal static Stack<CheckConstraint> ConstraintStack { get; set; }

        ///// <summary>
        ///// Returns a BooleanCheckConstraint that can use a function to evaluate
        ///// </summary>
        ///// <param name="evaluator">The function to use</param>
        ///// <returns>BooleanCheckConstraint</returns>
        //public static BooleanCheckConstraint IsTrue(Func<bool> evaluator)
        //{
        //    return new BooleanCheckConstraint(evaluator);
        //}

        /// <summary>
        /// Returns a BooleanCheckConstraint that can evaluate a boolean.
        /// </summary>
        /// <param name="condition">true/false</param>
        /// <returns>BooleanCheckConstraint</returns>
        public static BooleanCheckConstraint IsTrue(bool condition)
        {
            return new BooleanCheckConstraint(condition);
        }

        /// <summary>
        /// SuccessCheckConstraint that can evaluate the success or failure of an operation
        /// </summary>
        /// <typeparam name="T">Type the operation uses</typeparam>
        /// <param name="operation">the Operation to evaluate</param>
        /// <returns>SuccessCheckConstraint</returns>
        public static SuccessCheckConstraint<T> Successful<T>(IOperation<T> operation)
        {
            return new SuccessCheckConstraint<T>(operation);
        }

        /// <summary>
        /// Creates a Not operator that negates following constraints
        /// </summary>
        public static ConstraintOperator Not
        {
            get
            {
                var not = new NotConstraintOperator();
                ConstraintStack.Push(not);
                return not;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static BooleanCheckConstraint IsFalse(bool condition)
        {
            return Not.IsTrue(condition);
        }
    }
}
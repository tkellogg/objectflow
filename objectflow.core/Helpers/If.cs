using System;
using Rainbow.ObjectFlow.Constraints;
using Rainbow.ObjectFlow.Constraints.Operators;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Helpers
{
    ///<summary>
    ///</summary>
    public enum Step
    {
        ///<summary>
        ///</summary>
        Previous = 1
    }

    /// <summary>
    /// Helper class for building constraints
    /// </summary>
    public sealed class If
    {
        /// <summary>
        /// Returns a BooleanCheckConstraint that can use a function to evaluate
        /// </summary>
        /// <param name="evaluator">The function to use</param>
        /// <returns>BooleanCheckConstraint</returns>
        public static ICheckConstraint IsTrue(Func<bool> evaluator)
        {
            return new Condition(evaluator);
        }

        /// <summary>
        /// Returns a BooleanCheckConstraint that can use a function to evaluate
        /// </summary>
        /// <param name="evaluator">The function to use</param>
        /// <returns>BooleanCheckConstraint</returns>
        public static ICheckConstraint IsTrue(Func<bool> evaluator, IDeclaredOperation onFalse)
        {
            return new BranchCondition(evaluator, onFalse);
        }

        /// <summary>
        /// Returns a BooleanCheckConstraint that can evaluate a boolean.
        /// </summary>
        /// <param name="condition">true/false</param>
        /// <returns>BooleanCheckConstraint</returns>
        public static ICheckConstraint IsTrue(bool condition)
        {
            return new Condition(() => condition);
        }

        /// <summary>
        /// SuccessCheckConstraint that can evaluate the success or failure of an operation
        /// </summary>
        /// <typeparam name="T">Type the operation uses</typeparam>
        /// <param name="operation">the Operation to evaluate</param>
        /// <returns>SuccessCheckConstraint</returns>
        public static ICheckConstraint Successfull<T>(IOperation<T> operation)
        {
            Check.IsInstanceOf<BasicOperation<T>>(operation, "operation");

            return new Condition<BasicOperation<T>>((c) => c.SuccessResult, operation as BasicOperation<T>);
        }

        /// <summary>
        /// Creates a Not operator that negates following constraints
        /// </summary>
        public static ConstraintOperator Not
        {
            get
            {
                var not = new NotConstraintOperator();
                Dispatcher<ICheckConstraint>.ExecutionPlan.Push(not);
                return not;
            }
        }

        /// <summary>
        /// determines if the condition evaluates as false
        /// </summary>
        /// <param name="condition">condition to evalueate</param>
        /// <returns>True if the condition evaluates to false, false otherwise</returns>
        public static ICheckConstraint IsFalse(bool condition)
        {
            return Not.IsTrue(condition);
        }

        ///<summary>
        ///</summary>
        ///<param name="offset"></param>
        ///<returns></returns>
        public static ICheckConstraint Successfull(Step offset)
        {
            return null;
        }
    }
}
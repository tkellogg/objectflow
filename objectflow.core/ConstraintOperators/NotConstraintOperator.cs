namespace Rainbow.ObjectFlow.ConstraintOperators
{
    /// <summary>
    /// Negates following constraints.
    /// </summary>
    public sealed class NotConstraintOperator : ConstraintOperator
    {
        /// <summary>
        /// Evaluates the not expression.
        /// </summary>
        /// <param name="matches">The value to evaluate againt</param>
        /// <returns>True if equivalent, false otherise</returns>
        public override bool Matches(bool matches)
        {
            return !matches;
        }
    }
}
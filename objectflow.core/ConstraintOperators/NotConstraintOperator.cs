namespace Rainbow.ObjectFlow.ConstraintOperators
{
    /// <summary>
    /// Negates following constraints.
    /// </summary>
    public sealed class NotConstraintOperator : ConstraintOperator
    {
        public override bool Matches(bool matches)
        {
            return !matches;
        }
    }
}
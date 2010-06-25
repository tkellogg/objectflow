namespace Rainbow.ObjectFlow.Interfaces
{
    /// <summary>
    /// Pre-condition for workflow operations.
    /// </summary>
    public interface ICheckConstraint
    {
        /// <summary>
        /// Evaluates the constraint
        /// </summary>
        /// <returns>True if the constraint evaluated to true, false otherwise</returns>
        bool Matches();

        /// <summary>
        /// Evaluates the constraint
        /// </summary>
        /// <param name="match">Value we are checking it matching</param>
        /// <returns>True if the constraint evaluated to true, false otherwise</returns>
        bool Matches(bool match);
    }
}
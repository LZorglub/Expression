namespace Afk.Expression
{
    /// <summary>
    /// Implements expression type information
    /// </summary>
    interface ITypeExpression
    {
        /// <summary>
        /// Gets a value which indicates whether the expression is an boolean expression
        /// </summary>
        bool IsBooleanExpression { get; }
    }
}

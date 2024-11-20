namespace Afk.Expression
{
    /// <summary>
    /// Represents a custom operand
    /// </summary>
    public interface ICustomOperand
    {
        /// <summary>
        /// Manage a custom operation for this operand
        /// </summary>
        /// <param name="operator">Operator used</param>
        /// <param name="otherOperand">Second operand</param>
        /// <param name="isLeftOperand">Gets a value which indicates whether the custom operand is left or right</param>
        /// <returns>Operation result</returns>
        object HandleOperation(string @operator, object otherOperand, bool isLeftOperand);
    }
}

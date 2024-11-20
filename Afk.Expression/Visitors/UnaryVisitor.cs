namespace Afk.Expression
{
    /// <summary>
    /// Represents an unary node visitor
    /// </summary>
    public class UnaryVisitor
    {
        /// <summary>
        /// Initialize a new instance of <see cref="UnaryVisitor"/>
        /// </summary>
        /// <param name="node"></param>
        internal UnaryVisitor(UnaryNode node)
        {
            this.Method = node.Op.Op;
            this.Operand = node.Operand;
        }

        /// <summary>
        /// Gets the unary method
        /// </summary>
        public string Method { get; private set; }

        /// <summary>
        /// Gets the unary operand
        /// </summary>
        public object Operand { get; private set; }
    }
}

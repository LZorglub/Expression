namespace Afk.Expression
{
    /// <summary>
    /// Represents a binary node visitor
    /// </summary>
    public class BinaryVisitor
    {
        /// <summary>
        /// Initialize a new instance of <see cref="BinaryNode"/>
        /// </summary>
        /// <param name="node"></param>
        internal BinaryVisitor(BinaryNode node)
        {
            this.Method = node.Op.Op;
            this.Left = node.Operand1;
            this.Right = node.Operand2;
        }

        /// <summary>
        /// Gets the binary method
        /// </summary>
        public string Method { get; private set; }

        /// <summary>
        /// Gets the binary left operand
        /// </summary>
        public object Left { get; private set; }

        /// <summary>
        /// Gets the binary right operand
        /// </summary>
        public object Right { get; private set; }
    }
}

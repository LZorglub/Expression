namespace Afk.Expression
{
    /// <summary>
    /// Represents an user expression visitor
    /// </summary>
    public class UserVisitor
    {
        /// <summary>
        /// Initialize a new instance of <see cref="UserVisitor"/>
        /// </summary>
        /// <param name="node"></param>
        internal UserVisitor(UserExpression node)
        {
            this.Expression = node.Expression;
        }

        /// <summary>
        /// Gets the user expression
        /// </summary>
        public string Expression { get; private set; }
    }
}

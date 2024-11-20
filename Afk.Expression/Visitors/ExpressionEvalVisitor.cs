using System;

namespace Afk.Expression
{
    /// <summary>
    /// Represents a visitor of <see cref="ExpressionEval"/>
    /// </summary>
    public abstract class ExpressionEvalVisitor
    {
        /// <summary>
        /// Initialize a new instance of <see cref="ExpressionEvalVisitor"/>
        /// </summary>
        public ExpressionEvalVisitor()
        {

        }

        /// <summary>
        /// Visits a <see cref="ExpressionEval"/>
        /// </summary>
        /// <param name="node"></param>
        public virtual void Visit(ExpressionEval node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            object obj = node.Parse();
            Visit(obj);
        }

        /// <summary>
        /// Visits a node 
        /// </summary>
        /// <param name="node"></param>
        private void Visit(object node)
        {
            if (node is BinaryNode)
            {
                VisitBinaryExpression(new BinaryVisitor((BinaryNode)node));
            }
            else if (node is UnaryNode)
            {
                VisitUnaryExpression(new UnaryVisitor((UnaryNode)node));
            }
            else if (node is UserExpression)
            {
                VisitUserExpression(new UserVisitor((UserExpression)node));
            }
            else if (node is FunctionExpression)
            {
                throw new NotImplementedException();
            }
            else if (node is ArrayExpression)
            {
                throw new NotImplementedException();
            }
            else if (node is object[])
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Visits a binary node
        /// </summary>
        /// <param name="node"></param>
        protected virtual void VisitBinaryExpression(BinaryVisitor node)
        {
            Visit(node.Left);
            Visit(node.Right);
        }

        /// <summary>
        /// Visits an unary node
        /// </summary>
        /// <param name="node"></param>
        protected virtual void VisitUnaryExpression(UnaryVisitor node)
        {
            Visit(node.Operand);
        }

        /// <summary>
        /// Visits an user expression
        /// </summary>
        /// <param name="node"></param>
        protected virtual void VisitUserExpression(UserVisitor node)
        {

        }

        /// <summary>
        /// Visits an user function
        /// </summary>
        protected virtual void VisitFuncExpression()
        {

        }

        /// <summary>
        /// Visits an array
        /// </summary>
        protected virtual void VisitArrayExpression()
        {

        }
    }

}

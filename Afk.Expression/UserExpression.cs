using System;

namespace Afk.Expression
{
    /// <summary>
    /// Represents an user expression
    /// </summary>
    class UserExpression : IExpression
    {
        /// <summary>
        /// Represents the method that handles the user expression event
        /// </summary>
        public event UserExpressionEventHandler UserExpressionHandler;

        /// <summary>
        /// Initialize a new isntance of <see cref="UserExpression"/>
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="pow">Optional power</param>
        public UserExpression(string expression, double? pow = null)
        {
            this.Expression = expression;
            this.Pow = pow;
        }

        /// <summary>
        /// Gets the expression of <see cref="UserExpression"/>
        /// </summary>
        public string Expression
        {
            get;
        }

        /// <summary>
        /// Gets or sets optional pow
        /// </summary>
        public double? Pow { get; }

        /// <summary>
        /// Evalate the user expression
        /// </summary>
        /// <returns></returns>
        public object Evaluate()
        {
            return this.Evaluate(Guid.Empty);
        }

        /// <summary>
        /// Evalate the user expression
        /// </summary>
        /// <returns></returns>
        public object Evaluate(Guid correlationId)
        {
            if (UserExpressionHandler != null)
            {
                UserExpressionEventArgs e = new UserExpressionEventArgs(this.Expression, correlationId);
                UserExpressionHandler(this, e);
                if (e.Result != null && this.Pow != null) e.Result = Math.Pow(Convert.ToDouble(e.Result), this.Pow.Value); 
                return e.Result;
            }

            throw new ExpressionException(string.Format("Unable to evaluate expression {0}", this.Expression), 0, 0);
        }

        /// <summary>
        /// Reduce an user expression
        /// </summary>
        /// <returns></returns>
        public object Reduce() {
            // User expression can not be reduce, coz can change
            return this;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public string ToPolishString()
        {
            return "[UserExpr]";
        }
    }
}

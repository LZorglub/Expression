using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afk.Expression
{
    class UnaryNode : IExpression, ITypeExpression
    {
        /// <summary>
        /// Get the unary operator
        /// </summary>
        public UnaryOp Op { get; internal set; }

        /// <summary>
        /// Gets the unary operand
        /// </summary>
        public object Operand { get; internal set; }

        #region Membres de IExpression

        public string Expression
        {
            get
            {
                return null;
            }
        }

        public object Evaluate()
        {
            return this.Evaluate(null);
        }

        public object Evaluate(int? correlationId)
        {
            return Op.Do(Operand, correlationId);
        }

        /// <summary>
        /// Reduces the expression
        /// </summary>
        /// <returns></returns>
        public object Reduce()
        {
            IExpression temp = Operand as IExpression;
            if (temp != null)
            {
                object reduced = temp.Reduce();

                switch (Op.Op)
                {
                    case "+": return reduced;
                    case "-":
                        {
                            if (reduced is UnaryNode && ((UnaryNode)reduced).Op.Op == "-")
                                return ((UnaryNode)reduced).Operand;
                            else
                                return new UnaryNode() { Op = this.Op, Operand = reduced };
                        }
                    case "~":
                        return new UnaryNode() { Op = this.Op, Operand = reduced };
                    case "!":
                        {
                            return ExpressionInverser.LogicalInverse(reduced);
                        }
                }
            }
            else
            {
                return Evaluate();
            }
            throw new ArgumentException("Unary Operator " + Op + " not defined.");
        }
        #endregion

        /// <summary>
        /// Gets a value which indicates if unary node is boolean
        /// </summary>
        public bool IsBooleanExpression
        {
            get { return (this.Op.Op == "!"); }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public string ToPolishString()
        {
            if (this.Operand is IExpression)
            {
                return string.Format("{0} {1} ", this.Op.Op, ((IExpression) this.Operand).ToPolishString());
            }
            return string.Format("{0} {1} ", this.Op.Op, this.Operand);
        }
    }
}

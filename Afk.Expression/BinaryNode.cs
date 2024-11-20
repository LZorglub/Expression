using System;

namespace Afk.Expression
{
    class BinaryNode : IExpression, ITypeExpression
    {
        /// <summary>
        /// Gets or sets if binary node represents an entity (parenthesis)
        /// </summary>
        internal bool IsEntity { get; set; }

        /// <summary>
        /// Gets the binary operator
        /// </summary>
        public BinaryOp Op { get; internal set; }

        /// <summary>
        /// Gets the first operand
        /// </summary>
        public object Operand1 { get; internal set; }

        /// <summary>
        /// Gets the second operand
        /// </summary>
        public object Operand2 { get; internal set; }

        /// <summary>
        /// Gets the binary operator sensitivity
        /// </summary>
        public CaseSensitivity CaseSensitivity { get; internal set; }

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
            return this.Evaluate(Guid.Empty);
        }

        public object Evaluate(Guid correlationId)
        {
            return Op.DoBinaryOp(Operand1, Operand2, CaseSensitivity, correlationId);
        }

        /// <summary>
        /// Reduces a binary node
        /// </summary>
        /// <returns></returns>
        public object Reduce()
        {
            object op1 = this.Operand1;
            object op2 = this.Operand2;

            if (op1 is IExpression) op1 = ((IExpression)op1).Reduce();
            if (op2 is IExpression) op2 = ((IExpression)op2).Reduce();

            // If operands are not IExpression we can evaluate immediatly
            if (!(op1 is IExpression) && !(op2 is IExpression))
            {
                return Op.DoBinaryOp(op1, op2, CaseSensitivity, Guid.Empty);
            }

            return new BinaryNode()
            {
                IsEntity = this.IsEntity,
                CaseSensitivity = this.CaseSensitivity,
                Op = this.Op,
                Operand1 = op1,
                Operand2 = op2
            };
        }
        #endregion

        /// <summary>
        /// Gets a value which indicates if binary node is a boolean comparaison
        /// </summary>
        public bool IsBooleanExpression
        {
            get { return this.Op.IsBooleanOperator(); }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public string ToPolishString()
        {
            object op1 = (this.Operand1 is IExpression) ? ((IExpression)Operand1).ToPolishString() : Operand1;
            object op2 = (this.Operand2 is IExpression) ? ((IExpression)Operand2).ToPolishString() : Operand2;

            return string.Format("{0} {1} {2} ", this.Op.Op, op1, op2);
        }
    }
}

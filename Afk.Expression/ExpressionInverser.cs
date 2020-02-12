using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afk.Expression
{
    internal class ExpressionInverser
    {
        public static object LogicalInverse(object node)
        {
            if (node is BinaryNode)
            {
                return LogicalInverse((BinaryNode)node);
            }
            else if (node is UnaryNode)
            {
                return LogicalInverse((UnaryNode)node);
            }
            else
                throw new InvalidOperationException("Unable to inverse node");
        }

        private static object LogicalInverse(UnaryNode node)
        {
            switch (node.Op.Op)
            {
                case "+": 
                case "-": 
                case "~":
                    throw new InvalidOperationException("Inverse not defined on unary op " + node.Op.Op);
                case "!": 
                    return node.Operand;
            }
            throw new ArgumentException("Unary Operator " + node.Op.Op + " not defined.");
        }

        private static object LogicalInverse(BinaryNode node)
        {
            string inverseOp = null;

            switch (node.Op.Op.ToLower())
            {
                case "*":
                case "/":
                case "%":
                case "<<":
                case ">>":
                case "+":
                case "-":
                case "&":
                case "^":
                case "|":
                    inverseOp = node.Op.Op;
                    break;
                case "<":
                    inverseOp = ">=";
                    break;
                case "<=":
                    inverseOp = ">";
                    break;
                case ">":
                    inverseOp = "<=";
                    break;
                case ">=":
                    inverseOp = "<";
                    break;
                case "==":
                case "=":
                    inverseOp = "!=";
                    break;
                case "<>":
                case "!=":
                    inverseOp = "=";
                    break;
                case "and":
                case "&&":
                    inverseOp = "or";
                    break;
                case "or":
                case "||":
                    inverseOp = "and";
                    break;
            }
            if (inverseOp == null)
                throw new ArgumentException("Unable to inverse binary Operator " + node.Op.Op);

            var result = new BinaryNode()
            {
                IsEntity = node.IsEntity,
                Op = new BinaryOp(inverseOp, node.Op.OperatorType),
                CaseSensitivity = node.CaseSensitivity,
            };

            if (node.Operand1 is ITypeExpression && ((ITypeExpression)node.Operand1).IsBooleanExpression)
            {
                result.Operand1 = LogicalInverse(node.Operand1);
            }
            else
                result.Operand1 = node.Operand1;

            if (node.Operand2 is ITypeExpression && ((ITypeExpression)node.Operand2).IsBooleanExpression)
            {
                result.Operand2 = LogicalInverse(node.Operand2);
            }
            else
                result.Operand2 = node.Operand2;

            return result;
        }
    }
}

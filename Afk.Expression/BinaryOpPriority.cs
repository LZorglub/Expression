using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afk.Expression
{
    /// <summary>
    /// Provides priority on operator
    /// </summary>
    class BinaryOpPriority
    {
        /// <summary>
        /// Gets the priority of an operator
        /// </summary>
        /// <param name="op">Operator</param>
        /// <returns>Entier représentant le degré de priorité d'une opération binaire.
        /// Une valeur elevée représente un degré de priorité bas, la valeur 0 est la priorité
        /// maximale accordée à une opération.
        /// </returns>
        public static int GetPriority(string op)
        {
            switch (op.ToLower())
            {
                case "*":
                case "/":
                case "%": return 0;
                case "+":
                case "-": return 1;
                case ">>":
                case "<<": return 2;
                case "<":
                case "<=":
                case ">":
                case ">=": return 3;
                case "like":
                case "==":
                case "=":
                case "<>":
                case "!=": return 4;
                case "&": return 5;
                case "^": return 6;
                case "|": return 7;
                case "and":
                case "&&": return 8;
                case "or":
                case "||": return 9;
            }
            throw new ArgumentException("Operator " + op + "not defined.");
        }
    }
}

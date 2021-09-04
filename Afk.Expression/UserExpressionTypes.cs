using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afk.Expression
{
    /// <summary>
    /// Represents type of use expressions
    /// </summary>
    /// <remarks>Used to extract variables and functions in <see cref="ExpressionParser"/> extract method</remarks>
    [Flags]
    public enum UserExpressionTypes
    {
        /// <summary>
        /// Represent an user variable
        /// </summary>
        Variable = 0x01,
        /// <summary>
        /// Represent an user function
        /// </summary>
        Function = 0x02
    }
}

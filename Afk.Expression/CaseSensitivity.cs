using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afk.Expression
{
    /// <summary>
    /// Represents the sensitivity of expression
    /// </summary>
    [Flags]
    public enum CaseSensitivity
    {
        /// <summary>
        /// Not case sensitive
        /// </summary>
        None = 0x00,
        /// <summary>
        /// Sensitive to user expression
        /// </summary>
        UserExpression = 0x01,
        /// <summary>
        /// Sensitive to user constants
        /// </summary>
        UserConstants = 0x02,
        /// <summary>
        /// Sensitive to user functions
        /// </summary>
        UserFunction = 0x04,
        /// <summary>
        /// Sensitive to string
        /// </summary>
        String = 0x08,
    }
}

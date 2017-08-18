using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afk.Expression
{
    /// <summary>
    /// Represents an unitary operator
    /// </summary>
    struct UnaryOp
    {
        private string op;

        /// <summary>
        /// Gets the unary operator
        /// </summary>
        public string Op { get { return op; } }

        /// <summary>
        /// Initialize a new instance of <see cref="UnaryOp"/>
        /// </summary>
        /// <param name="op"></param>
        public UnaryOp(string op)
        {
           this.op = op;
        }

        /// <summary>
        /// Evaluate the unary operator
        /// </summary>
        /// <param name="v"></param>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        public object Do(object v, Guid correlationId)
        {
            IExpression tempv = v as IExpression;
            if (tempv != null)
                v = tempv.Evaluate(correlationId);

            switch (Op)
            {
                case "+": return (Convert.ToDouble(v, CultureInfo.InvariantCulture));
                case "-": return (-Convert.ToDouble(v, CultureInfo.InvariantCulture));
                case "!": return (!Convert.ToBoolean(v, CultureInfo.InvariantCulture));
                case "~": return (~Convert.ToUInt64(v, CultureInfo.InvariantCulture));
            }
            throw new ArgumentException("Unary Operator " + Op + " not defined.");
        }
    }
}

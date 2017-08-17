using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afk.Expression
{
    /// <summary>
    /// Represents an expression exception
    /// </summary>
    public class ExpressionException : Exception
    {
        private int _index;
        private int _length;
        private string _msg;

        /// <summary>
        /// Gets the index of exception
        /// </summary>
        public int Index
        {
            get { return _index; }
        }

        /// <summary>
        /// Gets the length of exception
        /// </summary>
        public int Length
        {
            get { return _length; }
        }

        /// <summary>
        /// Gets the exception message
        /// </summary>
        public override string Message
        {
            get
            {
                return _msg;
            }
        }

        /// <summary>
        /// Initialize a new instance of <see cref="ExpressionException"/>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        public ExpressionException(string msg, int index, int length)
        {
            _msg = msg;
            _index = index;
            _length = length;
        }
    }
}

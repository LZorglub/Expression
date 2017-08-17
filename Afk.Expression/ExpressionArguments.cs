using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afk.Expression
{
    /// <summary>
    /// Represents arguments of expression
    /// </summary>
    class ExpressionArguments : INotifyPropertyChanged
    {
        private List<string> variables;
        private List<string> functions;
        private Dictionary<string, object> constants;

        /// <summary>
        /// Initialize a new instance of <see cref="ExpressionArguments"/>
        /// </summary>
        public ExpressionArguments()
        {
            variables = new List<string>();
            functions = new List<string>();
            constants = new Dictionary<string, object>();
        }

        /// <summary>
        /// Adds a new variable in arguments list
        /// </summary>
        /// <param name="name"></param>
        public void AddVariable(string name)
        {
            variables.Add(name);
            this.OnPropertyChanged(nameof(Variables));
        }

        /// <summary>
        /// Add a new function in arguments list
        /// </summary>
        /// <param name="name"></param>
        public void AddFunctions(string name)
        {
            functions.Add(name);
            this.OnPropertyChanged(nameof(Functions));
        }

        /// <summary>
        /// Adds a new constant in argument list
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddConstants(string name, object value)
        {
            constants.Add(name, value);
            this.OnPropertyChanged("Constants");
        }

        /// <summary>
        /// Gets the variables name
        /// </summary>
        public IList<string> Variables { get { return variables.AsReadOnly(); } }

        /// <summary>
        /// Gets the functions name
        /// </summary>
        public IList<string> Functions { get { return functions.AsReadOnly(); } }

        /// <summary>
        /// Gets the constant name and value
        /// </summary>
        public IList<string> ConstantNames { get { return constants.Keys.ToList(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the constant value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="caseSensitivity"></param>
        /// <returns></returns>
        public object GetConstantValue(string name, CaseSensitivity caseSensitivity)
        {
            if ((caseSensitivity & Afk.Expression.CaseSensitivity.UserConstants) == Afk.Expression.CaseSensitivity.UserConstants)
            {
                // Case sensitive
                if (!this.constants.ContainsKey(name))
                    throw new ArgumentException(nameof(name));
                return this.constants[name];
            }
            else
            {
                // Case insensitive
                var ienum = this.constants.GetEnumerator();
                while (ienum.MoveNext())
                {
                    if (string.Compare(ienum.Current.Key, name, true) == 0)
                    {
                        return ienum.Current.Value;
                    }
                }
                throw new ArgumentException(nameof(name));
            }
        }

        /// <summary>
        /// Occurs when a property changed
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

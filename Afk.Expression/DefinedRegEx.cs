using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Afk.Expression
{
    class DefinedRegex
    {
        /// <summary>
        /// Defines the quote character
        /// </summary>
        internal const char QuoteCharacter = '\'';

        // Un numeric correspond à une partie entiérer facultative, suivi d'un point et éventuellement
        // d'une élévation à une puissance, il est forcement suivi d'un caractère non alphanumérique
        // Old value :  (?:[0-9]+)?(?:\.[0-9]+)?(?:E-?[0-9]+)?(?=\b) match E1 alone
        // This new one old 5E2 5.5E2 .5E2 but not E2 alone
        private const string c_strNumeric = @"(?:[0-9]+(?:\.[0-9]+)?|[0-9]*(?:\.[0-9]+){1}){1}(?:E-?[0-9]+)?(?=\b)";
        // Un booleen est soit true soit false
        private const string c_strBool = @"true|false";
        // Un héxadécimal est de la forme 0x suivi d'une série de caractère 0-9 A-F présent au moins une fois
        private const string c_strHex = @"0x([0-9a-fA-F]+)";

        // Une chaine de caractère est situé entre deux caractères ' pouvant contenir n'importe quel caractère
        // ainsi que le carctère ' doublé.
        private static readonly string c_strString = QuoteCharacter + @"(?<String>[^" + QuoteCharacter + "]*(?:" + QuoteCharacter + "{2}[^" + QuoteCharacter + "]*)*)" + QuoteCharacter;

        // Une date est de la forme YYYY-MM-DD avec l'heure optionnellement et un indicateur AM PM
        private const string c_strDate = @"\d{4}[-]\d{1,2}[-]\d{1,2}" +
            @"(?:\s+\d{1,2}\:\d{2}(?:\:\d{2})?\s*(?:AM|PM)?)?";

        // Le caractère whitespace est le caractère de séparation, il comprend:
        // space, tabs, new line, form feed, carriage return
        private const string c_strWhiteSpace = @"\s+";

        private const string stringNoCapture = 
            @"('[^']*(?:'{2}[^']*)*')+";

        // Un jeu de parenthese est composé d'une parenthèse ouvrante et se termine
        // à la n-ième parenthese fermante correspondant au n ouvrante. Les parenthéses
        // située dans les chaines de caractères ne sont pas comptabilisées.
        private const string c_strParenthesis =
            @"\G\((?<Parenthesis>" +
            @"(?>" + stringNoCapture + @"|[^()]|\((?<number>)|\)(?<-number>))*" +
            @"(?(number)(?!))" +
            @")\)";

        internal const string FunctionParameters =
            @"\((?<Parenthesis>" +
            @"(?>('[^']*(?:'{2}[^']*)*')+|[^()]|\((?<number>)|\)(?<-number>))*" +
            @"(?(number)(?!))" +
            @")\)";

        // Un tableau est composé d'un crochet ouvrant et se termine
        // au n-ième crochet fermant correspondant au n ouvrant. Les crochets
        // située dans les chaines de caractères ne sont pas comptabilisées.
        private const string c_strBrackets =
            @"\G\[(?<Bracket>" +
            @"(?>" + stringNoCapture + @"|[^\[\]]|\[(?<number>)|\](?<-number>))*" +
            @"(?(number)(?!))" +
            @")\]";

        // L'opérateur unaire est suivi d'un numeric ou d'une parenthèse, il est forcement
        // précédé par un opérateur binaire ou se trouve en début de chaine
        private const string c_strUnaryOp = @"(?:\+|-|!|~)(?=\w|\()";

        // Les opérateurs binaires sont des opérateurs de test ou d'opération
        // et ne doivent pas être précédés par un aute opérateur binaire
        private const string c_strBinaryOp = @"<<|>>|\+|-|\*|/|%|&&|\|\||&|\||\^|==|!=|<>|>=|<=|=|<|>|and(?=\b)|or(?=\b)|like(?=\b)";

        //		// Les éléments définis par l'utilisateur sont de type chaine commencant par
        //		// une lettre ou un signe $
        //		private const string c_strUserExpression  = @"([a-zA-Z$])+([a-zA-Z0-9$._+\-])*";

        // Comment on one line
        private const string c_strCommentLine = @"//.*$";

        // Comment on block
        private const string c_strCommentBlock = @"/\*.*?\*/";

        // Expressions régulières compilées

        internal static Regex Numeric = new Regex(
            c_strNumeric,
            RegexOptions.Compiled
        );

        internal static Regex Hexadecimal = new Regex(
            c_strHex,
            RegexOptions.Compiled
        );

        // Un booleen est case-insensitive
        internal static Regex Boolean = new Regex(
            c_strBool,
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        // Unaire forcement précédé par opérateur binaire ou en début de chaine
        internal static Regex UnaryOp = new Regex(
            @"(?<=(?:" + c_strBinaryOp + @")\s*|\A)(?:" + c_strUnaryOp + @")",
            RegexOptions.Compiled
        );

        // Binaire ne peut suivre un autre opérateur binaire et ne pas être en début de chaine
        internal static Regex BinaryOp = new Regex(
            @"(?<!(?:" + c_strBinaryOp + @")\s*|^\A)(?:" + c_strBinaryOp + @")",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        // Parenthèses
        internal static Regex Parenthesis = new Regex(
            c_strParenthesis,
            RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace
            );

        // Bracket
        internal static Regex Bracket = new Regex(
            c_strBrackets,
            RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace
            );

        //		// Données utilisateur
        //		internal static Regex UserExpression = new Regex(
        //			c_strUserExpression,
        //			RegexOptions.Compiled | RegexOptions.IgnoreCase
        //			);

        internal static Regex String = new Regex(
            c_strString,
            RegexOptions.Compiled
        );

        // Comment line
        internal static Regex CommentLine = new Regex(c_strCommentLine, RegexOptions.Compiled | RegexOptions.Multiline);

        // Comment line
        internal static Regex CommentBlock = new Regex(c_strCommentBlock, RegexOptions.Compiled | RegexOptions.Singleline);

        internal static Regex DateTime = new Regex(
            @"@D\((?<DateString>" + c_strDate + @")\)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
            );

        internal static Regex WhiteSpace = new Regex(
            c_strWhiteSpace,
            RegexOptions.Compiled
        );
    }
}

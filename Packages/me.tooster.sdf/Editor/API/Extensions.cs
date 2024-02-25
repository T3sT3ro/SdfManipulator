using System.Text.RegularExpressions;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.Editor.API {
    public static class Extensions {
        private static readonly Regex _invalidIdentifierCharactersRegex = new("[^a-zA-Z0-9_]", RegexOptions.Compiled);

        /// <summary>
        /// Creates a valid identifier from a given string:
        /// <ul>
        /// <li>Valid identifier has only alphanumeric and underscore characters and doesnt start with a number.</li>
        /// <li>Each invalid character is replaced with an underscore.</li>
        /// <li>If first character is a number, the identifier is prefixed with an underscore.</li>
        /// <li>If the string is empty, a single underscore is returned.</li>
        /// </ul>
        /// </summary>
        /// <param name="name">name to sanitize</param>
        /// <returns>a non-empty string</returns>
        public static string sanitizeToIdentifierString(this string name) {
            var s = _invalidIdentifierCharactersRegex.Replace(name, "_");
            if (s.Length == 0) return "_";
            return char.IsDigit(s[0]) ? $"_{s}" : s;
        }
    }
}

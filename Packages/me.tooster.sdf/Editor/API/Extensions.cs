using System.Text.RegularExpressions;
using me.tooster.sdf.AST.Syntax;
// using Unity.VisualScripting.YamlDotNet.Serialization.NamingConventions;

namespace me.tooster.sdf.Editor.API {
    public class Extensions {
        // TODO: remove dependency on VisualScripting package, implement own naming convention
        // public static readonly UnderscoredNamingConvention
        //     underscoreNamingFormatter = new UnderscoredNamingConvention();

        /// <summary>
        /// Returns a BEM-like string for element and block names (from most nested to least).
        /// It's up to the caller to ensure the validity of strings for their use case.
        /// </summary>
        /// <param name="element">final part of the string</param>
        /// <param name="blocks">a list of most to least nested blocks</param>
        /// <returns>string in format "outer_innner__element"</returns>
        public static string toBemString(string element, params string[] blocks) =>
            string.Join("_", blocks.AsReverseEnumerator()) + "__" + element;

        private static readonly Regex _sanitizeRegex = new Regex("[^a-zA-Z0-9_]", RegexOptions.Compiled);
        public static string sanitizeNameToIdentifier(string name) {
            var s = Regex.Replace(name, "[^a-zA-Z0-9_]", "_"); 
            return char.IsDigit(s[0]) ? $"_{s}" : s;
        }
    }
}

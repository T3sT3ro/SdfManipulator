using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
namespace me.tooster.sdf.Util {
    public static class Extensions {
        static readonly Regex  _invalidIdentifierCharactersRegex = new("[^a-zA-Z0-9_]", RegexOptions.Compiled);
        public static   string JoinToString<TItem>(this IEnumerable<TItem> items, string separator) => string.Join(separator, items);

        /// <summary>
        ///     Inverts a scale vector by dividing 1 by each component
        /// </summary>
        public static Vector3 Invert(this Vector3 vec) => new(1 / vec.x, 1 / vec.y, 1 / vec.z);

        public static IEnumerable<Transform> Ancestors(this Transform t) {
            while (t.parent != null) {
                t = t.parent;
                yield return t;
            }
        }

        public static IEnumerable<Transform> AncestorsAndSelf(this Transform t) {
            yield return t;
            foreach (var ancestor in t.Ancestors())
                yield return ancestor;
        }

        /// <summary>
        ///     Creates a valid identifier from a given string:
        ///     <ul>
        ///         <li>Valid identifier has only alphanumeric and underscore characters and doesnt start with a number.</li>
        ///         <li>Each invalid character is replaced with an underscore.</li>
        ///         <li>If first character is a number, the identifier is prefixed with an underscore.</li>
        ///         <li>If the string is empty, a single underscore is returned.</li>
        ///     </ul>
        /// </summary>
        /// <param name="name">name to sanitize</param>
        /// <returns>a non-empty string</returns>
        public static string sanitizeToIdentifierString(this string name) {
            var s = _invalidIdentifierCharactersRegex.Replace(name, "_");
            if (s.Length == 0)
                return "_";
            return char.IsDigit(s[0]) ? $"_{s}" : s;
        }
    }
}

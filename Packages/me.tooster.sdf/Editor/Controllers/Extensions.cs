using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Properties;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers {
    public static class Extensions {
        public static bool IsPropertyShaderlabCompatible(this IProperty p) {
            var t = p.DeclaredValueType();
            return
                t == typeof(int) || t == typeof(float)
             || t == typeof(Vector2) || t == typeof(Vector3) || t == typeof(Vector4)
             || t == typeof(Color)
             || t == typeof(Vector2Int) || t == typeof(Vector3Int)
             || t == typeof(Texture2D) || t == typeof(Texture2DArray) || t == typeof(Texture3D)
             || t == typeof(Cubemap) || t == typeof(CubemapArray);
        }

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

        static readonly Regex _invalidIdentifierCharactersRegex = new("[^a-zA-Z0-9_]", RegexOptions.Compiled);

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

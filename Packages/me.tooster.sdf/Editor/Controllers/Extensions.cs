using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using me.tooster.sdf.Editor.API;
using Unity.Properties;
using UnityEngine;
using Type = System.Type;

namespace me.tooster.sdf.Editor.Controllers {
    public static class Extensions {
        /// <summary>
        /// Collects all variables present on the node that should be exposed on the material as properties
        /// </summary>
        [Obsolete("this was taken from the previous graph model, not needed for now")]
        public static IEnumerable<Property> CollectProperties(object o) {
            return o.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(f => f.FieldType.IsSubclassOf(typeof(Property)))
                .SelectMany(f => (IEnumerable<Property>)f.GetValue(o))
                .Where(property => property is not null);
        }

        // TODO: remove or replace with getter instead of attribute?
        public static ISet<string> CollectIncludes(Type type) {
            return type.GetCustomAttributes(typeof(ShaderIncludeAttribute), true)
                .OfType<ShaderIncludeAttribute>()
                .SelectMany(attr => attr.ShaderIncludes)
                .Where(property => property is not null)
                .ToHashSet();
        }

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
    }
}

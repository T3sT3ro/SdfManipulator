using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using me.tooster.sdf.Editor.API;
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

        // TODO: remove or replace with getter instead of attribute?
        public static ISet<string> CollectDefines(object o) {
            // return values of static fields annotated with ShaderDefineAttribute
            return o.GetType().GetFields(BindingFlags.Static)
                .Where(info => info.GetCustomAttributes<ShaderGlobalAttribute>().Any())
                .Select(field => (string)field.GetValue(o))
                .Where(property => property is not null)
                .ToHashSet();
        }

        public static bool IsPropertyShaderlabCompatible(this Property p) => p
            is Property<int>
            or Property<float>
            or Property<Vector2>
            or Property<Vector3>
            or Property<Vector4>
            or Property<Color>
            or Property<Vector2Int>
            or Property<Vector3Int>
            or Property<Texture2D>
            or Property<Texture2DArray>
            or Property<Texture3D>
            or Property<Cubemap>
            or Property<CubemapArray>;

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

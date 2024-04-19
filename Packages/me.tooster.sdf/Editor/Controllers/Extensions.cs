using System.Collections.Generic;
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
    }
}

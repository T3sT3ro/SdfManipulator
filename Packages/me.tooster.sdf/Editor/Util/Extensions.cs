using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace me.tooster.sdf.Editor.Util {
    public static class Extensions {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source) {
            return source.Select((item, index) => (item, index));
        }

        public static string JoinToString<TItem>(this IEnumerable<TItem> items, string separator) => string.Join(separator, items);

        public static IEnumerable<T> GetImmediateChildrenComponents<T>(this GameObject parent) where T : Component
            => from Transform child in parent.transform select child.GetComponent<T>();

        /// <summary>
        /// Inverts a scale vector by dividing 1 by each component
        /// </summary>
        public static Vector3 Invert(this Vector3 vec) => new(1 / vec.x, 1 / vec.y, 1 / vec.z);
    }
}

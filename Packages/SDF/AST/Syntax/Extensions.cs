#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace AST.Syntax {
    public static class Extensions {
        /// Appends an item to an enumerable if it is not null.
        public static IEnumerable<T> AppendNotNull<T>(this IEnumerable<T> enumerable, T? item)
            where T : class =>
            item is null ? enumerable : enumerable.Append(item);

        public static IEnumerable<T> AppendAll<T>(this IEnumerable<T> enumerable, params T[] items)
            where T : class? =>
            enumerable.Concat(items);

        public static IEnumerable<T> AppendAll<T>(this IEnumerable<T> enumerable, IEnumerable<T> items)
            where T : class? =>
            enumerable.Concat(items);

        /// Concats an enumerable to another if it is not null.
        public static IEnumerable<T> ConcatNotNull<T>(this IEnumerable<T> enumerable, IEnumerable<T>? items)
            where T : class =>
            items is null ? enumerable : enumerable.Concat(items);

        public static IEnumerable<T> FilterNotNull<T>(this IEnumerable<T?> enumerable)
            where T : class =>
            enumerable.Where(i => i is not null).Select(i => i!);

        public static IEnumerable<(T, T)> ConsecutivePairs<T>(this IEnumerable<T> stream) {
            using var enumerator = stream.GetEnumerator();
            T last = enumerator.Current;
            while (enumerator.MoveNext()) {
                yield return (last, enumerator.Current);

                last = enumerator.Current;
            }
        }

        // Returns elements of collection but starting at index deletes deleteCount elements and inserts other elements
        public static IEnumerable<T> Splice<T>(this IEnumerable<T> self, int index, int deleteCount, IEnumerable<T> other) {
            var i = -1;
            foreach (var elem in self) {
                ++i;
                if (i == index) {
                    foreach (var inserted in other)
                        yield return inserted;
                } 
                
                if (index <= i && i < index + deleteCount) 
                    continue;
                
                yield return elem;
            }
        }
    }
}

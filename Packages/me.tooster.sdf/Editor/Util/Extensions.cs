using System.Collections.Generic;
using System.Linq;
namespace me.tooster.sdf.Editor.Util {
    public static class Extensions {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source) {
            return source.Select((item, index) => (item, index));
        }
    }
}

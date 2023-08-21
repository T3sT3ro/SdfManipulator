using System.Collections.Generic;
using System.Linq;

namespace API {
    // source: https://blog.yaakov.online/red-green-trees/
    sealed class RedGreenTree<T> {
        public RedGreenTree(GreenNode<T> rootNode) { this.rootNode = rootNode; }

        readonly GreenNode<T> rootNode;

        public RedNode<T> RootNode => new RedNode<T>(rootNode, null);
    }

    sealed class GreenNode<T> {
        public GreenNode(T value, IEnumerable<GreenNode<T>> children) {
            Value = value;
            Children = children;
        }

        public T                         Value    { get; }
        public IEnumerable<GreenNode<T>> Children { get; }
    }

    sealed class RedNode<T> {
        public RedNode(GreenNode<T> value, RedNode<T> parent) {
            this.value = value;
            Parent = parent;
        }

        readonly GreenNode<T> value;

        public T Value => value.Value;

        public IEnumerable<RedNode<T>> Children =>
            value.Children?.Select(c => new RedNode<T>(c, this)) ?? Enumerable.Empty<RedNode<T>>();

        public RedNode<T> Parent { get; }
    }
}

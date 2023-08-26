using System.Collections.Generic;
using System.Linq;

namespace AST.Syntax {
    // source: https://blog.yaakov.online/red-green-trees/
    sealed class RedGreenTree<T> {
        readonly GreenNode<T> rootNode;
        public RedGreenTree(GreenNode<T> rootNode) { this.rootNode = rootNode; }
        public RedNode<T> RootNode => new RedNode<T>(rootNode, null);
    }

    public class GreenNode {
        public IEnumerable<GreenNode> Children { get; }
        public GreenNode(IEnumerable<GreenNode> children) { Children = children; }
    }

    /// top-down tree node wrapper, immutable
    public sealed class GreenNode<T> {
        public T                         Value    { get; }
        public IEnumerable<GreenNode<T>> Children { get; }

        public GreenNode(T value, IEnumerable<GreenNode<T>> children) {
            Value = value;
            Children = children;
        }
    }

    /// readonly, bottom-up view of the tree, created dynamically during traversal
    public sealed class RedNode<T> {
        public   RedNode<T>   Parent { get; }
        readonly GreenNode<T> value;
        public   T            Value => value.Value;

        public RedNode(GreenNode<T> value, RedNode<T> parent) {
            this.value = value;
            Parent = parent;
        }

        public IEnumerable<RedNode<T>> Children =>
            value.Children?.Select(c => new RedNode<T>(c, this)) ?? Enumerable.Empty<RedNode<T>>();
    }
}

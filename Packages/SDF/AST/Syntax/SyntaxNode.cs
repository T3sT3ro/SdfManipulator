using System.Collections.Generic;

namespace AST.Syntax {
    public abstract record SyntaxNode<TNode, TBase>
        : ISyntaxNodeOrToken<TNode, TBase>
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase> {
        public          TNode        Parent              { get; internal set; }
        public abstract IList<TNode> ChildNodes          { get; }
        public abstract IList<TBase> ChildNodesAndTokens { get; }

        public IEnumerable<TNode> AncestorsAndSelf() {
            var node = (TNode)this;
            while (node != null) {
                yield return node;

                node = node.Parent;
            }
        }
    }
}

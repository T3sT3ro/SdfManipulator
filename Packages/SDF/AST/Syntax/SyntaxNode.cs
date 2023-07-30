#nullable enable
using System.Collections.Generic;
using System.Text;

namespace AST.Syntax {
    public abstract record SyntaxNode<TNode, TBase>
        : ISyntaxNodeOrToken<TNode, TBase>
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase> {
        public          TNode?               Parent              { get; internal set; }
        public abstract IReadOnlyList<TNode> ChildNodes          { get; }
        public abstract IReadOnlyList<TBase> ChildNodesAndTokens { get; }

        public IEnumerable<TNode> AncestorsAndSelf() {
            var node = (TNode)this;
            while (node != null) {
                yield return node;

                node = node.Parent;
            }
        }

        public void WriteTo(StringBuilder sb) {
            foreach (var child in ChildNodesAndTokens) {
                child.WriteTo(sb);
            }
        }

        public override string ToString() {
            var sb = new StringBuilder();
            WriteTo(sb);
            return sb.ToString();
        }
    }
}

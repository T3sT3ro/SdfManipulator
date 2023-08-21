using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AST.Syntax {
    public abstract record SyntaxNode<TNode, TBase>
        : ISyntaxNodeOrToken<TNode, TBase>
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase> {
        public          IReadOnlyList<TNode> ChildNodes          => ChildNodesAndTokens.OfType<TNode>().ToList();
        public abstract IReadOnlyList<TBase> ChildNodesAndTokens { get; }
        
        virtual public void WriteTo(StringBuilder sb) {
            foreach (var child in ChildNodesAndTokens)
                child.WriteTo(sb);
        }

        public override string ToString() {
            var sb = new StringBuilder();
            WriteTo(sb);
            return sb.ToString();
        }
    }
}

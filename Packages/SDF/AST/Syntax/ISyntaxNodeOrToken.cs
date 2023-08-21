using System.Text;

namespace AST.Syntax {
    public interface ISyntaxNodeOrToken<TNode, TBase>
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase> {

        public void WriteTo(StringBuilder sb);
    }
}

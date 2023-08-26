using System.Text;

namespace AST.Syntax {
    // parent class for syntax and tokens to represent lists of mixed types
    public interface ISyntaxNodeOrToken<TNode, TBase>
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase> {

        public void WriteTo(StringBuilder sb);
    }
}

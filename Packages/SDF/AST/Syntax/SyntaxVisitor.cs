#nullable enable
namespace AST.Syntax {
    public abstract class SyntaxVisitor<TResult, TNode, TToken, TBase, TTrivia>
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TToken : ISyntaxToken<TNode, TToken, TTrivia, TBase>, TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase>
        where TTrivia : SyntaxTrivia<TToken, TNode, TTrivia, TBase> {
        public virtual TResult? Visit(TNode node) { return default; }
    }

    public abstract class SyntaxVisitor<TNode, TToken, TBase, TTrivia> 
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TToken : ISyntaxToken<TNode, TToken, TTrivia, TBase>, TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase>
        where TTrivia : SyntaxTrivia<TToken, TNode, TTrivia, TBase> {
        public virtual void Visit(TNode node) { }
    }
}

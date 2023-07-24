namespace AST.Syntax {
    public interface ISyntaxTrivia<out TToken, TNode, TTrivia, TBase>
        where TToken : ISyntaxToken<TNode, TTrivia, TBase>, TBase
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase> 
        where TTrivia : ISyntaxTrivia<TToken, TNode, TTrivia, TBase> {
        /// token this trivia is attached to
        public TToken Token { get; }
    }
}

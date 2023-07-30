namespace AST.Syntax {
    public interface ISyntaxTrivia<TToken, TNode, TTrivia, TBase>
        where TToken : ISyntaxToken<TNode, TToken, TTrivia, TBase>, TBase
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase>
        where TTrivia : ISyntaxTrivia<TToken, TNode, TTrivia, TBase> {
        /// token this trivia is attached to
        public TToken Token { get; }

        public TNode  Structure { get; }
        public string Text      { get; }
    }
}

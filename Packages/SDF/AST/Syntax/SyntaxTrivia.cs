#nullable enable
namespace AST.Syntax {
    public abstract record SyntaxTrivia<TToken, TNode, TTrivia, TBase>
        where TToken : ISyntaxToken<TNode, TToken, TTrivia, TBase>, TBase
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase>
        where TTrivia : SyntaxTrivia<TToken, TNode, TTrivia, TBase> {
        /// token this trivia is attached to
        public TToken Token { get; set; }

        public TNode? Structure { get; set; }
        public string Text      { get; set; }
    }
}

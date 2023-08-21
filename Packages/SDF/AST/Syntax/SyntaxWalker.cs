namespace AST.Syntax {
    public abstract class SyntaxWalker<TNode, TToken, TBase, TTrivia>
        : SyntaxVisitor<TNode, TToken, TBase, TTrivia>
        where TBase : ISyntaxNodeOrToken<TNode, TBase>
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TToken : ISyntaxToken<TNode, TToken, TTrivia, TBase>, TBase
        where TTrivia : SyntaxTrivia<TToken, TNode, TTrivia, TBase> {
        protected SyntaxWalker() { }

        public override void Visit(TNode node) {
            foreach (var n in node.ChildNodesAndTokens)
                Visit((dynamic)n);
        }

        public virtual void Visit(TToken token) {
            foreach (var leadingTrivia in token.LeadingTrivia)
                Visit(leadingTrivia);

            foreach (var trailingTrivia in token.TrailingTrivia)
                Visit(trailingTrivia);
        }

        public virtual void Visit(TTrivia trivia) {
            if (trivia.Structure is not null)
                Visit(trivia.Structure);
        }
    }
}

namespace me.tooster.sdf.AST.Syntax {
    public abstract class SyntaxWalker<Lang> : SyntaxVisitor<Lang> {
        private bool DescentIntoStructuredTrivia { get; set; }

        protected SyntaxWalker(bool descendIntoStructuredTrivia = false) {
            DescentIntoStructuredTrivia = descendIntoStructuredTrivia;
        }


        protected override void Visit(Syntax<Lang> node) {
            foreach (var n in node.ChildNodesAndTokens)
                Visit((dynamic)n);
        }

        protected virtual void Visit(Token<Lang> token) {
            foreach (var leadingTrivia in token.LeadingTriviaList)
                Visit(leadingTrivia);

            foreach (var trailingTrivia in token.TrailingTriviaList)
                Visit(trailingTrivia);
        }

        protected virtual void Visit(Trivia<Lang> trivia) {
            if (trivia is StructuredTrivia<Lang> st && DescentIntoStructuredTrivia)
                Visit(st.Structure);
        }
    }
}

#nullable enable
namespace me.tooster.sdf.AST.Syntax {
    public abstract class Walker<Lang> : Visitor<Lang> {
        private bool DescentIntoStructuredTrivia { get; set; }

        protected Walker(bool descendIntoStructuredTrivia = false) {
            DescentIntoStructuredTrivia = descendIntoStructuredTrivia;
        }


        public override void Visit(Syntax<Lang>? node) {
            if (node == null) return;

            foreach (var n in node.ChildNodesAndTokens)
                Visit((dynamic)n);
        }

        public override void Visit(Token<Lang>? token) {
            if (token == null) return;

            if (token.LeadingTriviaList != null)
                foreach (var leadingTrivia in token.LeadingTriviaList)
                    Visit((dynamic)leadingTrivia);

            if (token.TrailingTriviaList != null)
                foreach (var trailingTrivia in token.TrailingTriviaList)
                    Visit((dynamic)trailingTrivia);
        }

        public override void Visit(TriviaList<Lang>? triviaList) {
            if (triviaList == null) return;

            foreach (var trivia in triviaList)
                Visit((dynamic)trivia);
        }

        public override void Visit(Trivia<Lang>? trivia) {
            if (trivia is StructuredTrivia<Lang> st && DescentIntoStructuredTrivia)
                Visit((dynamic)st.Structure);
        }
    }
}
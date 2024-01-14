#nullable enable

using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;

namespace me.tooster.sdf.AST {
    // instead of doing double dispatch using visit-accept pairs, pattern match can be used
    public abstract class Walker<Lang> : Visitor<Lang> {
        private bool DescentIntoStructuredTrivia { get; }

        protected Walker(bool descendIntoStructuredTrivia = false) =>
            DescentIntoStructuredTrivia = descendIntoStructuredTrivia;

        
        public void Visit(Anchor<Syntax<Lang>> a) {
            foreach (var n in a.Node.ChildNodesAndTokens)
                n.Accept(this, Anchor.New(n, a));
        }

        public void Visit(Anchor<SyntaxOrTokenList<Lang>> a) {
            foreach (var n in a.Node)
                n.Accept(this, Anchor.New(n, a));
        }

        public void Visit<T>(Anchor<SyntaxList<Lang, T>> a) where T : Syntax<Lang> {
            foreach (var n in a.Node) 
                n.Accept(this, Anchor.New(n, a));
        }

        public void Visit(Anchor<Token<Lang>> a) {
            a.Node.Accept(this, Anchor.New(a.Node.LeadingTriviaList, a));
            a.Node.Accept(this, Anchor.New(a.Node.TrailingTriviaList, a));
        }


        public void Visit(Anchor<TriviaList<Lang>> a) {
            foreach (var trivia in a.Node)
                trivia.Accept(this, Anchor.New(trivia, a));
        }

        public void Visit<T>(Anchor<StructuredTrivia<Lang>> a) {
            if (DescentIntoStructuredTrivia && a.Node.Structure is not null)
                a.Node.Accept(this, Anchor.New(a.Node, a));
        } 
        
        /*public void Visit<T>(Anchor<StructuredTrivia<Lang, T>> a) where T : Syntax<Lang> {
            if (DescentIntoStructuredTrivia && a.Node.Structure is not null)
                a.Node.Accept(this, Anchor.New(a.Node, a));
        }*/
    }
}

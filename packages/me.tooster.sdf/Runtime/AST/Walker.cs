#nullable enable

using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
namespace me.tooster.sdf.AST {
    /// <summary>
    ///     Visits every node of a tree depth-first, in syntax order: syntax nodes, lists, tokens and the trivia of tokens.
    ///     Override a <c>Visit</c> method to act on a node, and call the base method to visit its children.
    /// </summary>
    /// <remarks>
    ///     A walker that also implements a language visitor (e.g. <c>Hlsl.Visitor</c>) receives concrete nodes in the
    ///     language visitor's methods, which don't descend into children unless they call the methods below.
    /// </remarks>
    public abstract class Walker<Lang> : Visitor<Lang> {
        protected Walker(bool descendIntoStructuredTrivia = false) => DescendIntoStructuredTrivia = descendIntoStructuredTrivia;

        protected bool DescendIntoStructuredTrivia { get; }

        /// Visits <paramref name="root" /> and all of its descendants.
        public void Walk(Tree<Lang>.Node root) => root.Accept(this, null);

        // abstract node types dispatch to the concrete type
        public virtual void Visit(Anchor<Tree<Lang>.Node> a)    => a.Node.Accept(this, a.Parent);
        public virtual void Visit(Anchor<SyntaxOrToken<Lang>> a) => a.Node.Accept(this, a.Parent);
        public virtual void Visit(Anchor<Trivia<Lang>> a)        => a.Node.Accept(this, a.Parent);

        // syntax that a language visitor doesn't handle arrives as one of these
        public virtual void Visit(Anchor<Syntax<Lang>> a)          => VisitChildren(a);
        public virtual void Visit(Anchor<Statement<Lang>> a)       => VisitChildren(a);
        public virtual void Visit(Anchor<Expression<Lang>> a)      => VisitChildren(a);
        public virtual void Visit(Anchor<Identifier<Lang>> a)      => VisitChildren(a);
        public virtual void Visit(Anchor<CompilationUnit<Lang>> a) => VisitChildren(a);

        public virtual void Visit(Anchor<SyntaxOrTokenList<Lang>> a) => VisitChildren(a);

        public virtual void Visit<T>(Anchor<SyntaxList<Lang, T>> a) where T : Syntax<Lang> => VisitChildren(a);

        public virtual void Visit<T>(Anchor<SeparatedList<Lang, T>> a) where T : Syntax<Lang> => VisitChildren(a);

        // the injected tree is in another language, so only its start and end tokens are visited
        public virtual void Visit<T>(Anchor<InjectedLanguage<Lang, T>> a) => VisitChildren(a);

        public virtual void Visit(Anchor<Token<Lang>> a) => a.Node.LeadingTriviaList.Accept(this, a);

        public virtual void Visit(Anchor<TriviaList<Lang>> a) {
            foreach (var trivia in a.Node)
                trivia.Accept(this, a);
        }

        public virtual void Visit(Anchor<SimpleTrivia<Lang>> a) { }

        public virtual void Visit(Anchor<StructuredTrivia<Lang>> a) {
            if (DescendIntoStructuredTrivia)
                a.Node.Structure?.Accept(this, a);
        }

        /// Visits the children of <paramref name="a" />, in syntax order.
        protected void VisitChildren<T>(Anchor<T> a) where T : Syntax<Lang> {
            foreach (var child in a.Node.ChildNodesAndTokens())
                child.Accept(this, a);
        }
    }
}

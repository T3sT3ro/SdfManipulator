#nullable enable
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;

/*
 * There is no need for handling InjectedLanguage, because other language visitors may override
 * this themselves with specific language params
 */
namespace me.tooster.sdf.AST {
    // TODO: split into geenric AST node visitor and CommonSyntax visitor ?
    /// Visitor returning result for visited nodes
    public interface Visitor<Lang, out R> {
        public R? Visit(Anchor<Tree<Lang>.Node> a);

        public R? Visit(Anchor<SyntaxOrToken<Lang>> a);

        public R? Visit(Anchor<Syntax<Lang>> a);

        public R? Visit(Anchor<SyntaxOrTokenList<Lang>> a);

        public R? Visit<T>(Anchor<SyntaxList<Lang, T>> a) where T : Syntax<Lang>;

        public R? Visit<T>(Anchor<SeparatedList<Lang, T>> a) where T : Syntax<Lang>;

        public R? Visit(Anchor<Token<Lang>> a);

        public R? Visit(Anchor<Trivia<Lang>> a);

        public R? Visit(Anchor<TriviaList<Lang>> a);

        public R? Visit(Anchor<SimpleTrivia<Lang>> a);

        public R? Visit(Anchor<StructuredTrivia<Lang>> a);

        public R? Visit<T>(Anchor<InjectedLanguage<Lang, T>> a);

        public R? Visit(Anchor<Statement<Lang>> a);

        public R? Visit(Anchor<Expression<Lang>> a);

        public R? Visit(Anchor<Identifier<Lang>> a);

        public R? Visit(Anchor<CompilationUnit<Lang>> a);
    }



    /// Visitor without return values
    public interface Visitor<Lang> {
        // catch all
        public void Visit(Anchor<Tree<Lang>.Node> a) { }

        public void Visit(Anchor<SyntaxOrToken<Lang>> a) { }

        // syntax
        public void Visit(Anchor<Syntax<Lang>> a) { }

        // syntax lists
        public void Visit(Anchor<SyntaxOrTokenList<Lang>> a) { }

        public void Visit<T>(Anchor<SyntaxList<Lang, T>> a) where T : Syntax<Lang> { }

        public void Visit<T>(Anchor<SeparatedList<Lang, T>> a) where T : Syntax<Lang> { }

        // token
        public void Visit(Anchor<Token<Lang>> a) { }

        // trivia and trivia list
        public void Visit(Anchor<TriviaList<Lang>> a) { }

        public void Visit(Anchor<Trivia<Lang>> a) { }

        public void Visit(Anchor<SimpleTrivia<Lang>> a) { }

        public void Visit(Anchor<StructuredTrivia<Lang>> a) { }

        public void Visit<T>(Anchor<InjectedLanguage<Lang, T>> a) { }

        public void Visit(Anchor<Statement<Lang>> a) { }

        public void Visit(Anchor<Expression<Lang>> a) { }

        public void Visit(Anchor<Identifier<Lang>> a) { }

        public void Visit(Anchor<CompilationUnit<Lang>> a) { }
    }
}

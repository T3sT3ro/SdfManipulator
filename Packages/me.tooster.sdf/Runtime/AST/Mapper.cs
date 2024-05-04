#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
namespace me.tooster.sdf.AST {
    public abstract class Mapper<Lang> : Visitor<Lang, Tree<Lang>.Node> {
        protected bool descendIntoTrivia { get; init; } = false;

        protected Mapper(bool descendIntoTrivia = default) => this.descendIntoTrivia = descendIntoTrivia;

        public virtual Tree<Lang>.Node? Visit(Anchor<Token<Lang>> a) {
            var token = a.Node;
            var leading = token.LeadingTriviaList;
            if (descendIntoTrivia)
                leading = Visit(Anchor.New(leading, a)) as TriviaList<Lang>;

            // var trailing = token.TrailingTriviaList;
            // if (descendIntoTrivia)
            //     trailing = Visit(Anchor.New(trailing, a)) as TriviaList<Lang>;

            if (ReferenceEquals(token.LeadingTriviaList, leading)
                // && ReferenceEquals(token.TrailingTriviaList, trailing)
               )
                return token;

            return token with
            {
                LeadingTriviaList = leading ?? TriviaList<Lang>.Empty,
                // TrailingTriviaList = trailing ?? TriviaList<Lang>.Empty,
            };
        }

        public virtual Tree<Lang>.Node? Visit(Anchor<SyntaxOrTokenList<Lang>> a) {
            var newList = MapList<SyntaxOrTokenList<Lang>, SyntaxOrToken<Lang>>(a);
            return ReferenceEquals(newList, a.Node) ? a.Node : new SyntaxOrTokenList<Lang>(newList);
        }

        public virtual Tree<Lang>.Node? Visit<T>(Anchor<SyntaxList<Lang, T>> a) where T : Syntax<Lang> {
            var newList = MapList<SyntaxList<Lang, T>, T>(a);
            return ReferenceEquals(newList, a.Node) ? a.Node : new SyntaxList<Lang, T>(newList);
        }

        public virtual Tree<Lang>.Node? Visit(Anchor<TriviaList<Lang>> a) {
            var newList = MapList<TriviaList<Lang>, Trivia<Lang>>(a);
            return ReferenceEquals(newList, a.Node) ? a.Node : new TriviaList<Lang>(newList);
        }

        public virtual Tree<Lang>.Node? Visit<T>(Anchor<SeparatedList<Lang, T>> a) where T : Syntax<Lang> {
            var newList = MapList<SeparatedList<Lang, T>, SyntaxOrToken<Lang>>(a);
            return ReferenceEquals(newList, a.Node) ? a.Node : new SeparatedList<Lang, T>(newList);
        }

        // leaf - identity 
        public virtual Tree<Lang>.Node? Visit(Anchor<SimpleTrivia<Lang>> a) => a;

        public virtual Tree<Lang>.Node? Visit(Anchor<StructuredTrivia<Lang>> a) {
            var trivia = a.Node;
            if (trivia.Structure is null) return trivia;

            var newStructure = Visit(Anchor.New(trivia.Structure, a)) as Syntax<Lang>;
            return ReferenceEquals(trivia.Structure, newStructure)
                ? trivia
                : new StructuredTrivia<Lang> { Structure = newStructure };
        }

        // dispatch by concrete type - all of these are abstract so it's safe to do this if type hierarchy is properly set up
        public virtual Tree<Lang>.Node? Visit(Anchor<Tree<Lang>.Node> a) => a.Node.Accept(this, a.Parent);

        public virtual Tree<Lang>.Node? Visit(Anchor<SyntaxOrToken<Lang>> a)   => a.Node.Accept(this, a.Parent);
        public virtual Tree<Lang>.Node? Visit(Anchor<Syntax<Lang>> a)          => a.Node.Accept(this, a.Parent);
        public virtual Tree<Lang>.Node? Visit(Anchor<Trivia<Lang>> a)          => a.Node.Accept(this, a.Parent);
        public virtual Tree<Lang>.Node? Visit(Anchor<Statement<Lang>> a)       => a.Node.Accept(this, a.Parent);
        public virtual Tree<Lang>.Node? Visit(Anchor<Expression<Lang>> a)      => a.Node.Accept(this, a.Parent);
        public virtual Tree<Lang>.Node? Visit(Anchor<Identifier<Lang>> a)      => a.Node.Accept(this, a.Parent);
        public         Tree<Lang>.Node? Visit(Anchor<CompilationUnit<Lang>> a) => a.Node.Accept(this, a.Parent);

        // identity for injected language - requires custom processing
        public virtual Tree<Lang>.Node? Visit<T>(Anchor<InjectedLanguage<Lang, T>> a) => a;


        /// Returns new mapped list or old list if no element changed. Removes nulls from the mapped list
        protected IEnumerable<TElement> MapList<TList, TElement>(Anchor<TList> a)
            where TList : IEnumerable<TElement> where TElement : Tree<Lang>.Node {
            var changed = false;
            var newList = new List<TElement>();
            foreach (var element in a.Node) {
                if (element is null)
                    continue; // this happening is indication of a bad practice but it may happen nevertheless

                var newElement = element.Accept(this, Anchor.New(element, a)) as TElement;
                if (newElement is not null) // omit nulls
                    newList.Add(newElement);
                changed |= !ReferenceEquals(element, newElement);
            }

            return changed ? newList : a.Node;
        }
    }
}

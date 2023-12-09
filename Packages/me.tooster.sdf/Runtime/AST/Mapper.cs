#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST {
    public record MapperState {
        public readonly bool descendIntoTrivia = false;
    }

    public class Mapper<Lang, TOpts> : Visitor<Lang, Tree<Lang>.Node> where TOpts : MapperState, new() {
        protected readonly TOpts state;
        protected Mapper(TOpts? state) => this.state = state ?? new();

        // default visit -> return itself
        public virtual Tree<Lang>.Node? Visit(Anchor<Tree<Lang>.Node> a) => a.Node.Accept(this, Anchor.New(a.Node, a));
        public Tree<Lang>.Node? Visit(Anchor<SyntaxOrToken<Lang>> a) => a.Node.Accept(this, Anchor.New(a.Node, a.Parent));
        public Tree<Lang>.Node? Visit(Anchor<Syntax<Lang>> a) => a.Node.Accept(this, Anchor.New(a.Node, a.Parent));

        public Tree<Lang>.Node? Visit(Anchor<Token<Lang>> a) {
            var token = a.Node;
            var leading = token.LeadingTriviaList;
            if (leading is not null && state.descendIntoTrivia)
                leading = leading.Accept(this, Anchor.New(leading, a)) as TriviaList<Lang>;

            var trailing = token.TrailingTriviaList;
            if (trailing is not null && state.descendIntoTrivia)
                trailing = trailing.Accept(this, Anchor.New(trailing, a)) as TriviaList<Lang>;

            if (token.LeadingTriviaList == leading && token.TrailingTriviaList == trailing)
                return token;

            return token with
            {
                LeadingTriviaList = leading,
                TrailingTriviaList = trailing
            };
        }

        public Tree<Lang>.Node? Visit(Anchor<SyntaxOrTokenList<Lang>> a) {
            var newList = MapList<SyntaxOrTokenList<Lang>, SyntaxOrToken<Lang>>(a);
            return newList == a.Node ? a.Node : new SyntaxOrTokenList<Lang>(newList);
        }

        public Tree<Lang>.Node? Visit<T>(Anchor<SyntaxList<Lang, T>> a) where T : Syntax<Lang> {
            var newList = MapList<SyntaxList<Lang, T>, T>(a);
            return newList == a.Node ? a.Node : new SyntaxOrTokenList<Lang>(newList);
        }

        public Tree<Lang>.Node? Visit(Anchor<TriviaList<Lang>> a) {
            var newTriviaList = MapList<TriviaList<Lang>, Trivia<Lang>>(a);
            return newTriviaList == a.Node ? a.Node : new TriviaList<Lang>(newTriviaList);
        }

        public Tree<Lang>.Node? Visit<T>(Anchor<SeparatedList<Lang, T>> a) where T : Syntax<Lang> {
            var newList = MapList<SeparatedList<Lang, T>, SyntaxOrToken<Lang>>(a);
            return newList == a.Node ? a.Node : new SeparatedList<Lang, T>(newList);
        }

        public Tree<Lang>.Node? Visit(Anchor<SimpleTrivia<Lang>> a) => a.Node;

        public Tree<Lang>.Node? Visit<T>(Anchor<StructuredTrivia<Lang, T>> a) where T : SyntaxOrToken<Lang> {
            var trivia = a.Node;
            if (trivia.Structure is null) return trivia;

            var newStructure = a.Node.Accept(this, Anchor.New(trivia.Structure, a)) as T;
            return trivia.Structure == newStructure ? trivia : trivia with { Structure = newStructure };
        }

        // injected language is not processed by default
        public Tree<Lang>.Node? Visit<T>(Anchor<InjectedLanguage<Lang, T>> a) => a.Node;

        #region util

        /// Returns new mapped list or old list if no element changed. Removes nulls from the mapped list
        protected IEnumerable<TE> MapList<TL, TE>(Anchor<TL> a) where TL : IEnumerable<TE> where TE : Tree<Lang>.Node {
            var changed = false;
            var newList = new List<TE>();
            foreach (var element in a.Node) {
                var newElement = element.Accept(this, Anchor.New(element, a)) as TE;
                if (newElement is not null) // omit nulls
                    newList.Add(newElement);
                changed |= element != newElement;
            }

            return changed ? newList : a.Node;
        }

        #endregion
    }
}

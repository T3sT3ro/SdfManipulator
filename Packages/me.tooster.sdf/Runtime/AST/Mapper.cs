#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST {
    public partial class Mapper<Lang> {
        private bool DescentIntoTrivia { get; }

        public Mapper(bool descentIntoTrivia) { DescentIntoTrivia = descentIntoTrivia; }

        /* NOTES:
         - no default visit accepting object — if it doesn't exist it shouldn't work
        */

        // dispatch to source-generated methods
        public virtual Syntax<Lang>? Map(Syntax<Lang> syntax) => 
            syntax.MapWith(this);

        public virtual Syntax<Lang>? Map(Anchor<Syntax<Lang>> anchoredSyntax) =>
            anchoredSyntax.Node.MapWith(this, anchoredSyntax);


        // map token to new token and descend into trivia lists
        public virtual Token<Lang>? Map(Token<Lang> token) =>
            MapToken(token, null);

        public virtual Token<Lang>? Map(Anchor<Token<Lang>> anchoredToken) =>
            MapToken(anchoredToken.Node, anchoredToken);

        private Token<Lang> MapToken(Token<Lang> token, Anchor<Token<Lang>>? anchor) {
            var leading = token.LeadingTriviaList;
            if (leading is not null && DescentIntoTrivia)
                leading = Map((dynamic)(anchor is not null ? new Anchor<TriviaList<Lang>>(leading, anchor) : leading));

            var trailing = token.TrailingTriviaList;
            if (trailing is not null && DescentIntoTrivia)
                trailing = Map(
                    (dynamic)(anchor is not null ? new Anchor<TriviaList<Lang>>(trailing, anchor) : trailing));

            if (ReferenceEquals(token.LeadingTriviaList, leading)
             && ReferenceEquals(token.TrailingTriviaList, trailing))
                return token;

            return token with
            {
                LeadingTriviaList = leading is null ? leading : new TriviaList<Lang>(leading),
                TrailingTriviaList = trailing is null ? trailing : new TriviaList<Lang>(trailing)
            };
        }


        public virtual TriviaList<Lang>? Map(TriviaList<Lang> triviaList) =>
            MapTriviaList(triviaList);

        public virtual TriviaList<Lang>? Map(Anchor<TriviaList<Lang>> anchoredTriviaList) =>
            MapTriviaList(anchoredTriviaList.Node, anchoredTriviaList);

        private TriviaList<Lang> MapTriviaList(TriviaList<Lang> triviaList, Anchor<TriviaList<Lang>>? anchor = null) {
            var newTriviaList = MapList(triviaList, anchor);
            return ReferenceEquals(newTriviaList, triviaList) ? triviaList : new TriviaList<Lang>(newTriviaList);
        }


        public virtual StructuredTrivia<Lang, T>? Map<T>(StructuredTrivia<Lang, T> trivia)
            where T : SyntaxOrToken<Lang> =>
            MapStructuredTrivia(trivia);

        public virtual StructuredTrivia<Lang, T>? Map<T>(Anchor<StructuredTrivia<Lang, T>> anchoredTrivia)
            where T : SyntaxOrToken<Lang> =>
            MapStructuredTrivia(anchoredTrivia.Node, anchoredTrivia);

        private StructuredTrivia<Lang, T> MapStructuredTrivia<T>(
            StructuredTrivia<Lang, T> trivia,
            Anchor<StructuredTrivia<Lang, T>>? anchor = null
        ) where T : SyntaxOrToken<Lang> {
            if (trivia.Structure is null) return trivia;

            var newStructure =
                Map((dynamic)(anchor is null ? trivia.Structure : new Anchor<T>(trivia.Structure, anchor)));
            return ReferenceEquals(trivia.Structure, newStructure) ? trivia : trivia with { Structure = newStructure };
        }


        public virtual SimpleTrivia<Lang>? Map(SimpleTrivia<Lang> trivia) => trivia;

        public virtual SimpleTrivia<Lang>? Map(Anchor<SimpleTrivia<Lang>> anchoredTrivia) => anchoredTrivia.Node;


        public virtual SyntaxOrTokenList<Lang> Map(SyntaxOrTokenList<Lang> list) =>
            mapSyntaxOrTokenList(list);

        public virtual SyntaxOrTokenList<Lang> Map(Anchor<SyntaxOrTokenList<Lang>> anchoredList) =>
            mapSyntaxOrTokenList(anchoredList.Node, anchoredList);

        private SyntaxOrTokenList<Lang> mapSyntaxOrTokenList(SyntaxOrTokenList<Lang> list,
            Anchor<SyntaxOrTokenList<Lang>>? anchor = null) {
            var newList = MapList(list, anchor);
            return ReferenceEquals(newList, list) ? list : new SyntaxOrTokenList<Lang>(newList);
        }


        public virtual SyntaxList<Lang, T> Map<T>(SyntaxList<Lang, T> list) where T : Syntax<Lang> =>
            MapSyntaxList(list);

        public virtual SyntaxList<Lang, T> Map<T>(Anchor<SyntaxList<Lang, T>> anchoredList) where T : Syntax<Lang> =>
            MapSyntaxList(anchoredList.Node, anchoredList);
        
        private SyntaxList<Lang, T> MapSyntaxList<T>(SyntaxList<Lang, T> list, Anchor<SyntaxList<Lang, T>>? anchor = null)
            where T : Syntax<Lang> {
            var newList = MapList(list, anchor);
            return ReferenceEquals(newList, list) ? list : newList.Cast<T>().ToList();
        }

        
        public virtual SeparatedList<Lang, TSyntax> Map<TSyntax>(SeparatedList<Lang, TSyntax> list)
            where TSyntax : Syntax<Lang> {
            var newList = MapList(list);
            return ReferenceEquals(newList, list)
                ? list
                : new SeparatedList<Lang, TSyntax>((IReadOnlyList<TSyntax>)list);
        }

        /// Returns new mapped list or old list if no element changed
        protected IReadOnlyList<T> MapList<T>(IReadOnlyList<T> list, Anchor? anchor = null) {
            bool changed = false;
            var newList = new List<T>();
            foreach (var element in list) {
                var mappedElement = Map((dynamic)(anchor is null ? element : new Anchor<T>(element, anchor))!);
                newList.Add(mappedElement);
                changed |= !ReferenceEquals(element, mappedElement);
            }

            return changed ? newList : list;
        }
    }
}

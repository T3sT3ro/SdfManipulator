using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace me.tooster.sdf.AST.Syntax {
    public class Mapper<Lang> {
        private bool DescentIntoTrivia { get; }

        public Mapper(bool descentIntoTrivia) { DescentIntoTrivia = descentIntoTrivia; }

        // dispatch to source-generated methods
        public Syntax<Lang> Map(Syntax<Lang> syntax) => syntax.MapWith(this);

        // map token to new token and descend into trivia lists
        public Token<Lang> Map(Token<Lang> token) {
            var leading = Map((dynamic)token.LeadingTriviaList);
            var trailing = Map((dynamic)token.TrailingTriviaList);

            var leadingEqual = ReferenceEquals(token.LeadingTriviaList, leading);
            var trailingEqual = ReferenceEquals(token.TrailingTriviaList, trailing);

            if (leadingEqual && trailingEqual) return token;

            return token with
            {
                LeadingTriviaList = new TriviaList<Lang>(leading),
                TrailingTriviaList = new TriviaList<Lang>(trailing)
            };
        }

        public StructuredTrivia<Lang> Map(StructuredTrivia<Lang> trivia) {
            var newStructure = Map((dynamic)trivia.Structure);
            return ReferenceEquals(trivia.Structure, newStructure) ? trivia : trivia with { Structure = newStructure };
        }

        public SimpleTrivia<Lang> Map(SimpleTrivia<Lang> trivia) => trivia;

        public SyntaxOrTokenList<Lang> Map(SyntaxOrTokenList<Lang> list) {
            var newList = MapList(list);
            return ReferenceEquals(newList, list) ? list : new SyntaxOrTokenList<Lang>(newList);
        }

        public SyntaxList<Lang, TSyntax> Map<TSyntax>(SyntaxList<Lang, TSyntax> list) where TSyntax : Syntax<Lang> {
            var newList = MapList(list);
            return ReferenceEquals(newList, list) ? list : new SyntaxList<Lang, TSyntax>(newList.Cast<TSyntax>());
        }

        public SeparatedList<Lang, TSyntax> Map<TSyntax>(SeparatedList<Lang, TSyntax> list)
            where TSyntax : Syntax<Lang> {
            var newList = MapList(list);
            return ReferenceEquals(newList, list)
                ? list
                : new SeparatedList<Lang, TSyntax>((IReadOnlyList<TSyntax>)list);
        }

        private IReadOnlyList<T> MapList<T>(IReadOnlyList<T> list) {
            bool changed = false;
            var newList = new List<T>();
            foreach (var element in list) {
                var mappedElement = Map((dynamic)element);
                newList.Add(mappedElement);
                changed |= !ReferenceEquals(element, mappedElement);
            }

            return changed ? newList : list;
        }
    }
    // TODO: implement logic here instead of MapWith in separate classes??? but what about generator
}

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;


namespace me.tooster.sdf.AST {
    // TODO: this solution is suboptimal. It would be better to generate partial SyntaxRewriter methods for each Syntax type
    //       and Update methods inside syntax classes for updating partial fields of syntax tree parts, just like Roslyn
    [Obsolete]
    public class ReflectiveRewriter<Lang> : Visitor<Lang, Syntax<Lang>> {
        public bool DescendIntoStructuredTrivia { get; }

        public ReflectiveRewriter(bool descendIntoStructuredTrivia = false) {
            DescendIntoStructuredTrivia = descendIntoStructuredTrivia;
        }

        // ------------- ENTRYPOINT DISPATCH

        public override Syntax<Lang> Visit(Syntax<Lang>? node) {
            return Visit((dynamic)new Anchor<Syntax<Lang>>(node, null));
        }

        /* shouldn't be possible to visit tokens directly
        protected virtual Token<Lang> Visit(Token<Lang> token) {
            return Visit((dynamic)new WithParent<Token<Lang>>(token, null));
        }

        protected virtual Trivia<Lang> Visit(Trivia<Lang> trivia) {
            return Visit((dynamic)new WithParent<Trivia<Lang>>(trivia, null));
        }*/

        // ------------- DYNAMIC RUNTIME DISPATCH

        /// rewrites a syntax node. If it (or children) changed, returns changed node. Otherwise returns original node.
        protected virtual Syntax<Lang> Visit(Anchor<Syntax<Lang>> anchoredSyntax) {
            var syntax = anchoredSyntax.Node;
            Syntax<Lang>? modified = null; // will create clone only if children changed
            foreach (var p in syntax.GetType().GetProperties()
                         .Where(p => p.PropertyType.IsSubclassOf(typeof(SyntaxOrToken<Lang>)))) {
                var child = p.GetValue(syntax);
                object parentedChild = Activator.CreateInstance(typeof(Anchor).MakeGenericType(p.PropertyType),
                    child, anchoredSyntax);

                var newChild = Visit((dynamic)parentedChild);
                if (child == newChild)
                    continue;

                modified ??= syntax with { };     // shallow clone, preserves child identities if unmodified
                p.SetValue(modified, newChild); // set new child
            }

            return modified ?? syntax;
        }

        /// rewrites a token. If it (or children) changed, returns changed token. Otherwise returns original token.
        protected virtual Token<Lang> Visit(Anchor<Token<Lang>> anchoredToken) {
            var token = anchoredToken.Node;

            var leading = token.LeadingTriviaList;
            var trailing = token.TrailingTriviaList;

            var newLeading = Visit((dynamic)new Anchor<IReadOnlyList<Trivia<Lang>>>(leading, anchoredToken));
            var newTrailing = Visit((dynamic)new Anchor<IReadOnlyList<Trivia<Lang>>>(trailing, anchoredToken));

            if (newLeading != leading || newTrailing != trailing)
                return token with { LeadingTriviaList = newLeading, TrailingTriviaList = newTrailing };

            return token;
        }

        /// rewrite regular trivia, but there is nothing to do so return it
        protected virtual SimpleTrivia<Lang> Visit(Anchor<SimpleTrivia<Lang>> anchoredTrivia) {
            return anchoredTrivia.Node;
        }

        /*
        /// rewrites a trivia. If it (or children) changed, returns changed trivia. Otherwise returns original trivia.
        protected virtual Trivia<Lang> Visit(Anchor<StructuredTrivia<Lang>> triviaWithParent) {
            var trivia = triviaWithParent.Node;
            if (!DescendIntoStructuredTrivia) 
                return trivia;
            var result = Visit((dynamic)trivia.Structure);
            if (result == trivia) 
                return trivia;
            
            return trivia with { Structure = result };
        }
        */

        /// rewrites list's elements, if it (or children) changed, returns changed list. Othwerwise returns original list.
        protected virtual IReadOnlyList<T> Visit<T>(Anchor<IReadOnlyList<T>> anchoredList) where T : class {
            var list = anchoredList.Node;

            var newList =
                (IList<T>)Activator.CreateInstance(typeof(Anchor<>).MakeGenericType(list.GetType()), list);
            bool anyChanged = false;
            foreach (T x in list) {
                var parented = Activator.CreateInstance(typeof(Anchor).MakeGenericType(x.GetType()), x,
                    anchoredList);
                T mapped = (T)Visit((dynamic)parented);
                newList.Add(mapped);
                anyChanged |= mapped != x;
            }

            return anyChanged ? (IReadOnlyList<T>)newList : list;
        }
    }
}

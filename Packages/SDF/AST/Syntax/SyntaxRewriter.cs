#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace AST.Syntax {
    // TODO: this solution is suboptimal. It would be better to generate partial SyntaxRewriter methods for each Syntax type
    //       and Update methods inside syntax classes for updating partial fields of syntax tree parts, just like Roslyn
    public class SyntaxRewriter<Lang> : SyntaxVisitor<Lang, Syntax<Lang>> {
        public bool DescendIntoStructuredTrivia { get; }

        public SyntaxRewriter(bool descendIntoStructuredTrivia = false) {
            DescendIntoStructuredTrivia = descendIntoStructuredTrivia;
        }

        // ------------- ENTRYPOINT DISPATCH
        
        protected override Syntax<Lang> Visit(Syntax<Lang> node) {
            return Visit((dynamic)new WithParent<dynamic>(node, null));
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
        protected virtual Syntax<Lang> Visit(WithParent<Syntax<Lang>> nodeWithParent) {
            var node = nodeWithParent.Value;
            Syntax<Lang>? modified = null; // will create clone only if children changed
            foreach (var p in node.GetType().GetProperties()
                         .Where(p => p.PropertyType.IsSubclassOf(typeof(SyntaxOrToken<Lang>)))) {
                var child = p.GetValue(node);
                object parentedChild = Activator.CreateInstance(typeof(WithParent<>).MakeGenericType(p.PropertyType),
                    child, nodeWithParent);

                var newChild = Visit((dynamic)parentedChild);
                if (child == newChild)
                    continue;

                modified ??= node with { }; // shallow clone, preserves child identities if unmodified
                p.SetValue(modified, newChild); // set new child
            }

            return modified ?? node;
        }

        /// rewrites a token. If it (or children) changed, returns changed token. Otherwise returns original token.
        protected virtual Token<Lang> Visit(WithParent<Token<Lang>> tokenWithParent) {
            var token = tokenWithParent.Value;
            
            var leading = token.LeadingTriviaList;
            var trailing = token.TrailingTriviaList;
            
            var newLeading = Visit((dynamic)new WithParent<IReadOnlyList<Trivia<Lang>>>(leading, tokenWithParent));
            var newTrailing = Visit((dynamic)new WithParent<IReadOnlyList<Trivia<Lang>>>(trailing, tokenWithParent));
            
            if(newLeading != leading || newTrailing != trailing)
                return token with { LeadingTriviaList = newLeading, TrailingTriviaList = newTrailing };

            return token;
        }

        /// rewrite regular trivia, but there is nothing to do so return it
        protected virtual Trivia<Lang> Visit(WithParent<Trivia<Lang>> triviaWithParent) {
            return triviaWithParent.Value;
        }

        /// rewrites a trivia. If it (or children) changed, returns changed trivia. Otherwise returns original trivia.
        protected virtual Trivia<Lang> Visit(WithParent<StructuredTrivia<Lang>> triviaWithParent) {
            var trivia = triviaWithParent.Value;
            if (!DescendIntoStructuredTrivia) 
                return trivia;
            var result = Visit((dynamic)trivia.Structure);
            if (result == trivia) 
                return trivia;
            
            return trivia with { Structure = result };
        }

        /// rewrites list's elements, if it (or children) changed, returns changed list. Othwerwise returns original list.
        protected virtual IReadOnlyList<T> Visit<T>(WithParent<IReadOnlyList<T>> listWithParent) where T : class {
            var list = listWithParent.Value;
            
            var newList = (IList<T>)Activator.CreateInstance(typeof(WithParent<>).MakeGenericType(list.GetType()), list);
            bool anyChanged = false;
            foreach (T x in list) {
                var parented = Activator.CreateInstance(typeof(WithParent<>).MakeGenericType(x.GetType()), x, listWithParent);
                T mapped = (T)Visit((dynamic)parented);
                newList.Add(mapped);
                anyChanged |= mapped != x;
            }

            return anyChanged ? (IReadOnlyList<T>)newList : list;
        }

        // ------------- HELPERS 
        
        protected class DynamicParentNode {
            public DynamicParentNode? Parent { get; }

            protected DynamicParentNode(DynamicParentNode? parent) { Parent = parent; }
        }

        // TODO: get rid of it, use red-green tree like in Roslyn, auto-generated from annotations
        // Instantiating would be possible only with https://stackoverflow.com/questions/8718199/passing-a-type-to-a-generic-constructor
        protected sealed class WithParent<T> : DynamicParentNode {
            public T Value { get; }
            public WithParent(T value, DynamicParentNode? parent) : base(parent) { Value = value; }
        }
    }
}

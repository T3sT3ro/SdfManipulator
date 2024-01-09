#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST {
    // TODO: move to Zipper data structure with necessary extensions
    public static class Navigation {
        public static IEnumerable<IAnchor> Ancestors(this IAnchor node) {
            var current = node;
            while (current.Parent is not null) {
                yield return current.Parent;

                current = current.Parent;
            }
        }

        public enum EdgeChild { FIRST, LAST }
        
        public static Token<TLang>? FirstToken<TLang>(this IAnchor<SyntaxOrToken<TLang>> a) =>
            a.EdgeToken(EdgeChild.FIRST);

        public static Token<TLang>? LastToken<TLang>(this IAnchor<SyntaxOrToken<TLang>> a) =>
            a.EdgeToken(EdgeChild.LAST);

        private static Token<TLang>? EdgeToken<TLang>(this IAnchor<SyntaxOrToken<TLang>> a, EdgeChild edgeChild) {
            var stack = new Stack<SyntaxOrToken<TLang>>();
            stack.Push(a.Node);
            while (stack.Count > 0) {
                var current = stack.Pop();

                if (current is Token<TLang> tok) return tok;

                var childrenOrdered = ((Syntax<TLang>)current).ChildNodesAndTokens;
                foreach (var child in edgeChild != EdgeChild.FIRST ? childrenOrdered : childrenOrdered.Reverse())
                    stack.Push(child);
            }

            return null;
        }

        // from https://github.com/dotnet/roslyn/blob/462e180642875c0540ae1379e60425f635ec4f78/src/Compilers/Core/Portable/Syntax/SyntaxNavigator.cs#L435
        public static Token<Lang>? NextToken<Lang>(this IAnchor<Token<Lang>> aToken) {
            // onlly internal nodes are derivations of Syntax and StructuredTrivia
            IAnchor<SyntaxOrToken<Lang>>? lookingFor = aToken;
            var parent = aToken.Parent;
            while (parent is { Node: Syntax<Lang> syntax }) {
                var returnNext = false;
                foreach (var child in syntax.ChildNodesAndTokens) {
                    if (returnNext) {
                        if (child is Token<Lang> tok) return tok;

                        return Anchor.New((Syntax<Lang>)child, parent).FirstToken();
                    }

                    if (ReferenceEquals(child, lookingFor?.Node)) {
                        returnNext = true;
                    }
                }

                // when we reach final token in list we have to search for this syntax in parent
                if (returnNext) lookingFor = parent as IAnchor<Syntax<Lang>>;
                parent = parent.Parent;
            }

            // from https://github.com/dotnet/roslyn/blob/462e180642875c0540ae1379e60425f635ec4f78/src/Compilers/Core/Portable/Syntax/SyntaxNavigator.cs#L311
            return default; // TODO: implement this for structured trivia
        }
    }
}

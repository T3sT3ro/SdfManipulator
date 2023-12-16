#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST {
    public static class Navigation {
        
        
        public static IEnumerable<IAnchor> Ancestors(IAnchor node) {
            var current = node;
            while (current.Parent is not null) {
                yield return current.Parent;
                current = current.Parent;
            }
        }
        
        public static Token<TLang>? getFirstToken<TLang, T>(IAnchor<T> syntax) where T : Syntax<TLang>{
            var stack = new Stack<SyntaxOrToken<TLang>>();
            stack.Push(syntax.Node);
            while (stack.Count > 0) {
                var current = stack.Pop();
                
                if (current is Token<TLang> tok) return tok;
                foreach(var child in ((Syntax<TLang>)current).ChildNodesAndTokens.Reverse())
                    stack.Push(child);
            }
            return null;
        }

        public static Token<TLang>? getLastToken<TLang, T>(IAnchor<T> syntax) where T : Syntax<TLang> {
            var stack = new Stack<SyntaxOrToken<TLang>>();
            stack.Push(syntax.Node);
            while (stack.Count > 0) {
                var current = stack.Pop();

                if (current is Token<TLang> tok) return tok;
                foreach (var child in ((Syntax<TLang>)current).ChildNodesAndTokens)
                    stack.Push(child);
            }
            return null;
        }
        
        // from https://github.com/dotnet/roslyn/blob/462e180642875c0540ae1379e60425f635ec4f78/src/Compilers/Core/Portable/Syntax/SyntaxNavigator.cs#L435
        public static Token<Lang>? getNextToken<Lang, T>(IAnchor<T> aToken) where T : Token<Lang> {
            // onlly internal nodes are derivations of Syntax and StructuredTrivia
            IAnchor<SyntaxOrToken<Lang>> lookingFor = aToken;
            var parent = aToken.Parent;
            while (parent is {Node: Syntax<Lang> syntax} ) {
                var returnNext = false;
                foreach (var child in syntax.ChildNodesAndTokens) {
                    if (returnNext) {
                        if (child is Token<Lang> tok) return tok;
                        return getFirstToken<Lang, Syntax<Lang>>(Anchor.New((Syntax<Lang>)child, parent));
                    }

                    if (ReferenceEquals(child, lookingFor.Node)) {
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
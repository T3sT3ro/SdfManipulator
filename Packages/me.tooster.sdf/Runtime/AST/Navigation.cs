#nullable enable
using System.Collections.Generic;
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



        /// subtree side 
        public enum Side { LEFT = +1, RIGHT = -1 }



        public static Side Opposite(this Side side) => (Side)(-(int)side);


        public static Anchor<Token<TLang>>? FirstToken<TLang>(
            this IAnchor<SyntaxOrToken<TLang>> a,
            Side side = Side.LEFT
        ) {
            var stack = new Stack<IAnchor<SyntaxOrToken<TLang>>>();
            stack.Push(a);
            while (stack.Count > 0) {
                var current = stack.Pop();

                if (current is { Node: Token<TLang> tok }) return Anchor.New(tok, current.Parent);

                var childrenOrdered = ((Syntax<TLang>)current.Node).ChildNodesAndTokens();
                foreach (var child in side != Side.LEFT
                             ? childrenOrdered
                             : childrenOrdered.AsReverseEnumerator())
                    stack.Push(Anchor.New(child, (Anchor?)current));
            }

            return null;
        }

        /// returns neighboring token
        /// <remarks>based on https://github.com/dotnet/roslyn/blob/462e180642875c0540ae1379e60425f635ec4f78/src/Compilers/Core/Portable/Syntax/SyntaxNavigator.cs#L435</remarks>
        public static Anchor<Token<Lang>>? NextToken<Lang>(
            this IAnchor<Token<Lang>> aToken,
            Side to = Side.RIGHT
        ) {
            // only internal nodes are derivations of Syntax and StructuredTrivia
            IAnchor<SyntaxOrToken<Lang>>? lookingFor = aToken;
            var parent = aToken.Parent;
            while (parent is { Node: Syntax<Lang> syntax }) {
                var returnNext = false;
                foreach (var child in to is Side.RIGHT
                             ? syntax.ChildNodesAndTokens()
                             : syntax.ChildNodesAndTokens().AsReverseEnumerator()) {
                    if (returnNext) {
                        if (child is Token<Lang> tok) return Anchor.New(tok, parent);

                        if (Anchor.New((Syntax<Lang>)child, parent).FirstToken(to.Opposite()) is { } firstToken)
                            return firstToken;
                    } else if (ReferenceEquals(child, lookingFor?.Node))
                        returnNext = true;
                }

                // when we reach final token in list we have to search for this syntax in parent
                if (returnNext) lookingFor = parent as IAnchor<SyntaxOrToken<Lang>>;
                parent = parent.Parent;
            }

            // from https://github.com/dotnet/roslyn/blob/462e180642875c0540ae1379e60425f635ec4f78/src/Compilers/Core/Portable/Syntax/SyntaxNavigator.cs#L311
            return default; // TODO: implement this for structured trivia
        }

        public static IEnumerable<IAnchor> ParentsWithTokenOnEdge<Lang>(
            IAnchor<Token<Lang>> aToken,
            Side side = Side.LEFT
        ) {
            IAnchor lookingFor = aToken;
            var edgeIndex = side == Side.LEFT ? 0 : ^1;
            IAnchor? parent = aToken.Parent;
            while (parent is not null) {
                switch (parent) {
                    case { Node: StructuredTrivia<Lang> }:
                    case { Node: Syntax<Lang> s } when ReferenceEquals(lookingFor.Node, s.ChildNodesAndTokens()[edgeIndex]):
                    case { Node: TriviaList<Lang> triviaList } when ReferenceEquals(lookingFor.Node, triviaList[edgeIndex]):
                        yield return parent;
                        break;
                }
                lookingFor = parent;
                parent = lookingFor.Parent;
            }
        }
    }
}

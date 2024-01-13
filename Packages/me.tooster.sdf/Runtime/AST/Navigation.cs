#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;

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

        /// Represents syntax visiting direction, FORWARD for reading from the beginning to end, BACKWARD from end to beginning
        public enum Direction { FORWARD, BACKWARD }

        public static Anchor<Token<TLang>>? LastToken<TLang>(this IAnchor<SyntaxOrToken<TLang>> a) =>
            a.FirstToken(Direction.BACKWARD);

        public static Anchor<Token<TLang>>? FirstToken<TLang>(this IAnchor<SyntaxOrToken<TLang>> a,
            Direction direction = Direction.FORWARD) {
            var stack = new Stack<IAnchor<SyntaxOrToken<TLang>>>();
            stack.Push(a);
            while (stack.Count > 0) {
                var current = stack.Pop();

                if (current is {Node: Token<TLang> tok}) return Anchor.New(tok, current.Parent);

                var childrenOrdered = ((Syntax<TLang>)current.Node).ChildNodesAndTokens;
                foreach (var child in direction != Direction.FORWARD
                             ? childrenOrdered
                             : childrenOrdered.AsReverseEnumerator())
                    stack.Push(Anchor.New(child, (Anchor?)current));
            }

            return null;
        }

        /// returns neighboring token
        /// <remarks>based on https://github.com/dotnet/roslyn/blob/462e180642875c0540ae1379e60425f635ec4f78/src/Compilers/Core/Portable/Syntax/SyntaxNavigator.cs#L435</remarks>
        public static Anchor<Token<Lang>>? NextToken<Lang>(this IAnchor<Token<Lang>> aToken,
            Direction direction = Direction.FORWARD) {
            // only internal nodes are derivations of Syntax and StructuredTrivia
            IAnchor<SyntaxOrToken<Lang>>? lookingFor = aToken;
            var parent = aToken.Parent;
            while (parent is { Node: Syntax<Lang> syntax }) {
                var returnNext = false;
                foreach (var child in direction is Direction.FORWARD
                             ? syntax.ChildNodesAndTokens
                             : syntax.ChildNodesAndTokens.AsReverseEnumerator()) {
                    if (returnNext) {
                        if (child is Token<Lang> tok) return Anchor.New(tok, parent);

                        if (Anchor.New((Syntax<Lang>)child, parent).FirstToken(direction) is { } firstToken)
                            return firstToken;
                    } else if (ReferenceEquals(child, lookingFor?.Node)) {
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

        public static Anchor<Token<Lang>>? PreviousToken<Lang>(this IAnchor<Token<Lang>> aToken) =>
            aToken.NextToken(Direction.BACKWARD);
    }
}

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using me.tooster.sdf.AST.Shaderlab;
using me.tooster.sdf.AST.Syntax.CommonSyntax;


namespace me.tooster.sdf.AST.Syntax {
    public static class Extensions {
        /// Appends an item to an enumerable if it is not null.
        public static IEnumerable<T> AppendNotNull<T>(this IEnumerable<T> enumerable, T? item)
            where T : class =>
            item is null ? enumerable : enumerable.Append(item);

        public static IEnumerable<T> AppendAll<T>(this IEnumerable<T> enumerable, IEnumerable<T> items)
            where T : class? =>
            enumerable.Concat(items);

        /// Concats an enumerable to another if it is not null.
        public static IEnumerable<T> ConcatNotNull<T>(this IEnumerable<T> enumerable, IEnumerable<T>? items)
            where T : class =>
            items is null ? enumerable : enumerable.Concat(items);

        public static IEnumerable<T> FilterNotNull<T>(this IEnumerable<T?> enumerable)
            where T : class =>
            enumerable.Where(i => i is not null).Select(i => i!);

        public static IEnumerable<(T, T)> ConsecutivePairs<T>(this IEnumerable<T> stream) {
            using var enumerator = stream.GetEnumerator();
            T last = enumerator.Current;
            while (enumerator.MoveNext()) {
                yield return (last, enumerator.Current);

                last = enumerator.Current;
            }
        }

        public static IEnumerable<T> AsReverseEnumerator<T>(this IReadOnlyList<T> list) {
            for (int i = list.Count; --i >= 0;) yield return list[i];
        }

        public static int FindLastIndex<T>(this IReadOnlyList<T> list, Func<T, bool> predicate) {
            for (var i = list.Count; --i >= 0;)
                if (predicate(list[i]))
                    return i;

            return -1;
        }

        public static int LastIndexOf<T>(this IReadOnlyList<T> list, T element) where T : IEquatable<T> {
            for (var i = list.Count; --i >= 0;)
                if (EqualityComparer<T>.Default.Equals(list[i], element))
                    return i;

            return -1;
        }

        /**
         * <summary>Returns elements of collection but starting at index deletes deleteCount elements and inserts other elements</summary>
         * <param name="self">base enumerable to act upon</param>
         * <param name="index">index where splicing should occur</param>
         * <param name="deleteCount">number of elements that will be deleted starting from index</param>
         * <param name="other">elements which will get inserted starting at index (previous elements starting at index are pushed back)</param>
         * <returns>new enumerable with elements changed accordingly</returns>
         */
        public static IEnumerable<T> Splice<T>(
            this IEnumerable<T> self,
            int index,
            int deleteCount,
            IEnumerable<T> other
        ) {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (deleteCount < 0)
                throw new ArgumentOutOfRangeException(nameof(deleteCount), "Delete count cannot be negative.");

            using var enumerator = self.GetEnumerator();
            var i = 0;

            while (i < index && enumerator.MoveNext()) {
                yield return enumerator.Current;

                i++;
            }

            foreach (var inserted in other) yield return inserted;

            while (i < index + deleteCount && enumerator.MoveNext()) i++;

            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }

        /** <inheritdoc cref="Splice{T}(IEnumerable{T}, int, int, IEnumerable{T})"/> */
        public static IEnumerable<T>
            Splice<T>(this IEnumerable<T> self, int index, int deleteCount, params T[] other) =>
            Splice(self, index, deleteCount, other.AsEnumerable());

        public static IEnumerable<Syntax<Lang>> DescendantNodes<Lang>(this Syntax<Lang> root) =>
            root.DescendantNodesAndSelf().Skip(1);

        public static IEnumerable<Syntax<Lang>> DescendantNodesAndSelf<Lang>(this Syntax<Lang> root) {
            var stack = new Stack<Syntax<Lang>>();
            stack.Push(root);

            while (stack.Count > 0) {
                var current = stack.Pop();
                yield return current;

                foreach (var child in current.ChildNodes.Reverse())
                    stack.Push(child);
            }
        }

        public static IEnumerable<SyntaxOrToken<Lang>> DescendantNodesAndTokens<Lang>(this Syntax<Lang> root) {
            var stack = new Stack<SyntaxOrToken<Lang>>();
            stack.Push(root);

            while (stack.Count > 0) {
                var current = stack.Pop();
                yield return current;

                if (current is not Syntax<Lang> node) continue;

                foreach (var child in node.ChildNodesAndTokens.Reverse())
                    stack.Push(child);
            }
        }

        // similar to https://github.com/dotnet/roslyn/blob/main/src/Compilers/CSharp/Portable/Syntax/InternalSyntax/SyntaxLastTokenReplacer.cs
        // and https://github.com/dotnet/roslyn/blob/main/src/Compilers/CSharp/Portable/Syntax/InternalSyntax/SyntaxFirstTokenReplacer.cs
        internal class EdgeTokenReplacer<Lang> : Mapper<Lang, MapperState> {
            private readonly Token<Lang> oldToken;
            private readonly Token<Lang> newToken;
            private          bool        found;

            internal static TSyntax Replace<TSyntax>(
                TSyntax root,
                Token<Lang> oldToken,
                Token<Lang> newToken
            ) where TSyntax : Syntax<Lang> {
                var replacer = new EdgeTokenReplacer<Lang>(oldToken, newToken);
                var replaced = replacer.Visit(Anchor.New<Syntax<Lang>>(root));
                return (TSyntax)(replaced ?? root);
            }

            private EdgeTokenReplacer(Token<Lang> oldToken, Token<Lang> newToken) {
                this.oldToken = oldToken;
                this.newToken = newToken;
            }

            public override Tree<Lang>.Node? Visit(Anchor<Trivia<Lang>> a) {
                if (found || a.Node != oldToken) return a.Node;

                found = true;
                return newToken;
            }
        }

        public static TriviaList<Lang>? LeadingTriviaList<TSyntax, Lang>(this TSyntax syntax)
            where TSyntax : Syntax<Lang> => Anchor.New(syntax).FirstToken()?.Node.LeadingTriviaList;

        public static TriviaList<Lang>? TrailingTriviaList<TSyntax, Lang>(this TSyntax syntax)
            where TSyntax : Syntax<Lang> => Anchor.New(syntax).LastToken()?.Node.TrailingTriviaList;

        public static TSyntax WithLeadingTriviaList<TSyntax, Lang>(this TSyntax syntax, TriviaList<Lang> triviaList)
            where TSyntax : Syntax<Lang> =>
            WithTriviaList(syntax, triviaList, Navigation.Direction.FORWARD);
        
        public static TSyntax WithLeadingTrivia<TSyntax, Lang>(this TSyntax syntax, params Trivia<Lang>[] trivia)
            where TSyntax : Syntax<Lang> =>
            WithTriviaList(syntax, new TriviaList<Lang>(trivia), Navigation.Direction.FORWARD);
        
        public static TSyntax WithTrailingTriviaList<TSyntax, Lang>(this TSyntax syntax, TriviaList<Lang> triviaList)
            where TSyntax : Syntax<Lang> =>
            WithTriviaList(syntax, triviaList, Navigation.Direction.BACKWARD);
        
        public static TSyntax WithTrailingTrivia<TSyntax, Lang>(this TSyntax syntax, params Trivia<Lang>[] trivia)
            where TSyntax : Syntax<Lang> =>
            WithTriviaList(syntax, new TriviaList<Lang>(trivia), Navigation.Direction.BACKWARD);

        private static TSyntax WithTriviaList<TSyntax, Lang>(
            this TSyntax syntax,
            TriviaList<Lang> triviaList,
            Navigation.Direction direction
        ) where TSyntax : Syntax<Lang> {
            var oldToken = direction is Navigation.Direction.FORWARD
                ? Anchor.New(syntax).FirstToken()
                : Anchor.New(syntax).LastToken();

            if (oldToken is null) return syntax;

            var newToken = direction is Navigation.Direction.FORWARD
                ? oldToken.Node with { LeadingTriviaList = triviaList }
                : oldToken.Node with { TrailingTriviaList = triviaList };

            return EdgeTokenReplacer<Lang>.Replace(syntax, oldToken, newToken);
        }

        public static StructuredTrivia<Lang> ToStructuredTrivia<Lang>(this Syntax<Lang> syntax) =>
            new StructuredTrivia<Lang> { Structure = syntax };
        
        public static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

        public static string Repeat(this string s, uint n) => string.Concat(Enumerable.Repeat(s, (int)n));
    }
}

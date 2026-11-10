#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using me.tooster.sdf.AST.Shaderlab;
namespace me.tooster.sdf.AST.Syntax {
    public static class Extensions {
        /// <summary>
        ///     Formats <paramref name="value" /> as a decimal number with a decimal point and without an exponent, e.g.
        ///     <c>1.0</c>, <c>-0.5</c> or <c>0.0000001</c>. It uses the shortest digits that read back as the same float.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">for NaN and infinity, which shader languages have no literal for</exception>
        public static string ToDecimalLiteralText(this float value) {
            if (float.IsNaN(value) || float.IsInfinity(value))
                throw new ArgumentOutOfRangeException(nameof(value), value, "A shader literal can't represent NaN or infinity.");

            var roundTrip = value.ToString("R", CultureInfo.InvariantCulture); // e.g. "1.5", "-1.5E-07" or "3.402823E+38"
            var negative = roundTrip.StartsWith("-", StringComparison.Ordinal);
            var unsigned = negative ? roundTrip.Substring(1) : roundTrip;

            var exponentIndex = unsigned.IndexOfAny(new[] { 'E', 'e' });
            var mantissa = exponentIndex < 0 ? unsigned : unsigned.Substring(0, exponentIndex);
            var exponent = exponentIndex < 0 ? 0 : int.Parse(unsigned.Substring(exponentIndex + 1), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);

            var pointIndex = mantissa.IndexOf('.');
            var digits = pointIndex < 0 ? mantissa : mantissa.Remove(pointIndex, 1);
            var integerDigitCount = (pointIndex < 0 ? mantissa.Length : pointIndex) + exponent;

            var text = integerDigitCount <= 0
                ? "0." + new string('0', -integerDigitCount) + digits
                : integerDigitCount >= digits.Length
                    ? digits + new string('0', integerDigitCount - digits.Length) + ".0"
                    : digits.Substring(0, integerDigitCount) + "." + digits.Substring(integerDigitCount);

            return negative ? "-" + text : text;
        }

        // empty whitespace is redundant
        public static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);
        /// Appends an item to an enumerable if it is not null.
        public static IEnumerable<T> AppendNotNull<T>(this IEnumerable<T> enumerable, T? item)
            where T : class
            => item is null ? enumerable : enumerable.Append(item);

        /// Concats an enumerable to another if it is not null.
        public static IEnumerable<T> ConcatNotNull<T>(this IEnumerable<T> enumerable, IEnumerable<T>? items)
            where T : class
            => items is null ? enumerable : enumerable.Concat(items);

        public static IEnumerable<T> FilterNotNull<T>(this IEnumerable<T?> enumerable)
            where T : class
            => enumerable.Where(i => i is not null).Select(i => i!);

        public static IEnumerable<(T, T)> ConsecutivePairs<T>(this IEnumerable<T> stream) {
            using var enumerator = stream.GetEnumerator();
            var last = enumerator.Current;
            while (enumerator.MoveNext()) {
                yield return (last, enumerator.Current);

                last = enumerator.Current;
            }
        }

        public static IEnumerable<T> AsReverseEnumerator<T>(this IReadOnlyList<T> list) {
            for (var i = list.Count; --i >= 0;)
                yield return list[i];
        }

        public static int FindLastIndex<T>(this IReadOnlyList<T> list, Func<T, bool> predicate) {
            for (var i = list.Count; --i >= 0;) {
                if (predicate(list[i]))
                    return i;
            }

            return -1;
        }

        public static int LastIndexOf<T>(this IReadOnlyList<T> list, T element) where T : IEquatable<T> {
            for (var i = list.Count; --i >= 0;) {
                if (EqualityComparer<T>.Default.Equals(list[i], element))
                    return i;
            }

            return -1;
        }

        /**
         * <summary>Returns elements of collection but starting at index deletes deleteCount elements and inserts other elements</summary>
         * <param name="self">base enumerable to act upon</param>
         * <param name="index">index where splicing should occur</param>
         * <param name="deleteCount">number of elements that will be deleted starting from index</param>
         * <param name="other">
         *     elements which will get inserted starting at index (previous elements starting at index are pushed
         *     back)
         * </param>
         * <returns>new enumerable with elements changed accordingly</returns>
         */
        public static IEnumerable<T> Splice<T>(
            this IEnumerable<T> self,
            int index,
            int deleteCount,
            IEnumerable<T> other
        ) {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            if (deleteCount < 0)
                throw new ArgumentOutOfRangeException(nameof(deleteCount), "Delete count cannot be negative.");

            using var enumerator = self.GetEnumerator();
            var i = 0;

            while (i < index && enumerator.MoveNext()) {
                yield return enumerator.Current;

                i++;
            }

            foreach (var inserted in other)
                yield return inserted;

            while (i < index + deleteCount && enumerator.MoveNext())
                i++;

            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }

        /**
         * <inheritdoc cref="Splice{T}(IEnumerable{T}, int, int, IEnumerable{T})" />
         */
        public static IEnumerable<T>
            Splice<T>(this IEnumerable<T> self, int index, int deleteCount, params T[] other)
            => self.Splice(index, deleteCount, other.AsEnumerable());

        public static IEnumerable<Syntax<Lang>> DescendantNodes<Lang>(this Syntax<Lang> root) => root.DescendantNodesAndSelf().Skip(1);

        public static IEnumerable<Syntax<Lang>> DescendantNodesAndSelf<Lang>(this Syntax<Lang> root) {
            var stack = new Stack<Syntax<Lang>>();
            stack.Push(root);

            while (stack.Count > 0) {
                var current = stack.Pop();
                yield return current;

                foreach (var child in current.ChildNodes().Reverse())
                    stack.Push(child);
            }
        }

        public static IEnumerable<SyntaxOrToken<Lang>> DescendantNodesAndTokens<Lang>(this Syntax<Lang> root) {
            var stack = new Stack<SyntaxOrToken<Lang>>();
            stack.Push(root);

            while (stack.Count > 0) {
                var current = stack.Pop();
                yield return current;

                if (current is not Syntax<Lang> node)
                    continue;

                foreach (var child in node.ChildNodesAndTokens().Reverse())
                    stack.Push(child);
            }
        }



        public static TriviaList<Lang>? LeadingTriviaList<Lang>(this Syntax<Lang> syntax)
            => Anchor.New(syntax).FirstToken()?.Node.LeadingTriviaList;

        /*public static TriviaList<Lang>? TrailingTriviaList<Lang>(this Syntax<Lang> syntax)
            => Anchor.New(syntax).LastToken()?.Node.TrailingTriviaList;*/

        public static TSyntax WithLeadingTriviaList<TSyntax, Lang>(this TSyntax syntax, TriviaList<Lang>? triviaList)
            where TSyntax : Syntax<Lang>
            => syntax.WithTriviaList(triviaList /*, Navigation.Direction.FORWARD*/);

        public static TSyntax WithLeadingTrivia<TSyntax, Lang>(this TSyntax syntax, params Trivia<Lang>[] trivia)
            where TSyntax : Syntax<Lang>
            => syntax.WithTriviaList(new TriviaList<Lang>(trivia) /*, Navigation.Direction.FORWARD*/);

        public static TSyntax WithLeadingTrivia<TSyntax, Lang>(this TSyntax syntax, IEnumerable<Trivia<Lang>> trivia)
            where TSyntax : Syntax<Lang>
            => syntax.WithTriviaList(new TriviaList<Lang>(trivia) /*, Navigation.Direction.FORWARD*/);

        // public static TSyntax WithTrailingTriviaList<TSyntax, Lang>(this TSyntax syntax, TriviaList<Lang>? triviaList)
        //     where TSyntax : Syntax<Lang>
        //     => WithTriviaList(syntax, triviaList, Navigation.Direction.BACKWARD);
        //
        // public static TSyntax WithTrailingTrivia<TSyntax, Lang>(this TSyntax syntax, params Trivia<Lang>[] trivia)
        //     where TSyntax : Syntax<Lang>
        //     => WithTriviaList(syntax, new TriviaList<Lang>(trivia), Navigation.Direction.BACKWARD);
        //
        // public static TSyntax WithTrailingTrivia<TSyntax, Lang>(this TSyntax syntax, IEnumerable<Trivia<Lang>> trivia)
        //     where TSyntax : Syntax<Lang>
        //     => WithTriviaList(syntax, new TriviaList<Lang>(trivia), Navigation.Direction.BACKWARD);

        static TSyntax WithTriviaList<TSyntax, Lang>(
            this TSyntax syntax,
            TriviaList<Lang>? triviaList /*,
            Navigation.Direction direction*/
        ) where TSyntax : Syntax<Lang> {
            var oldToken = /*direction is Navigation.Direction.FORWARD
                ?*/ Anchor.New(syntax).FirstToken()
                /*: Anchor.New(syntax).LastToken()*/;

            if (oldToken is null)
                return syntax;

            triviaList ??= TriviaList<Lang>.Empty;

            var newToken = /*direction is Navigation.Direction.FORWARD
                ? */oldToken.Node with { LeadingTriviaList = triviaList }
                /*: oldToken.Node with { TrailingTriviaList = triviaList }*/;

            return ((syntax, oldToken.Node, newToken) switch
            {
                (Syntax<shaderlab> s, Token<shaderlab> tOld, Token<shaderlab> tNew) =>
                    new ShaderlabEdgeTokenReplacer(tOld, tNew).Visit(Anchor.New(s)) as TSyntax,
                (Syntax<hlsl> s, Token<hlsl> tOld, Token<hlsl> tNew) =>
                    new HlslEdgeTokenReplacer(tOld, tNew).Visit(Anchor.New(s)) as TSyntax,
                _ => throw new ArgumentException("Unhandled node type"),
            })!;
        }

        public static StructuredTrivia<Lang> ToStructuredTrivia<Lang>(this Syntax<Lang> syntax)         => new() { Structure = syntax };
        public static TriviaList<Lang>       AsTriviaList<Lang>(this IEnumerable<Trivia<Lang>> trivias) => new(trivias);

        public static string Repeat(this string s, uint n) => string.Concat(Enumerable.Repeat(s, (int)n));



        /// similar to
        /// https://github.com/dotnet/roslyn/blob/main/src/Compilers/CSharp/Portable/Syntax/InternalSyntax/SyntaxLastTokenReplacer.cs
        /// and
        /// https://github.com/dotnet/roslyn/blob/main/src/Compilers/CSharp/Portable/Syntax/InternalSyntax/SyntaxFirstTokenReplacer.cs
        /// TODO: decouple from Hlsl and Shaderlab after syntax-tree refactor
        class EdgeTokenReplacerState<Lang> {
            internal readonly Token<Lang> newToken;
            internal readonly Token<Lang> oldToken;
            bool                          found;

            internal EdgeTokenReplacerState(Token<Lang> oldToken, Token<Lang> newToken) {
                this.oldToken = oldToken;
                this.newToken = newToken;
            }

            internal Token<Lang> TryReplace(Token<Lang> node) {
                if (found || node != oldToken)
                    return node;

                found = true;
                return newToken;
            }
        }



        class ShaderlabEdgeTokenReplacer : Mapper {
            readonly EdgeTokenReplacerState<shaderlab> state;

            public ShaderlabEdgeTokenReplacer(Token<shaderlab> oldToken, Token<shaderlab> newToken)
                => state = new EdgeTokenReplacerState<shaderlab>(oldToken, newToken);

            public override Tree<shaderlab>.Node? Visit(Anchor<Token<shaderlab>> a) {
                var replaced = state.TryReplace(a.Node);
                return ReferenceEquals(replaced, state.newToken) ? replaced : base.Visit(a);
            }
        }



        class HlslEdgeTokenReplacer : Hlsl.Mapper {
            readonly EdgeTokenReplacerState<hlsl> state;

            public HlslEdgeTokenReplacer(Token<hlsl> oldToken, Token<hlsl> newToken)
                => state = new EdgeTokenReplacerState<hlsl>(oldToken, newToken);

            public override Tree<hlsl>.Node? Visit(Anchor<Token<hlsl>> a) {
                var replaced = state.TryReplace(a.Node);
                return ReferenceEquals(replaced, state.newToken) ? replaced : base.Visit(a);
            }
        }
    }
}

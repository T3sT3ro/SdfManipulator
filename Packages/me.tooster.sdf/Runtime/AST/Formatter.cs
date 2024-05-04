#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonTrivia;
namespace me.tooster.sdf.AST {
    /// <summary>
    /// Mutable state of the formatter to support line breaks and indents
    /// </summary>
    public record FormatterState {
        string        SingleIndent { get; init; }        = "    ";
        public string IndentString { get; private set; } = "";
        int           indentLevel = 0;


        /// <summary>
        /// Gets or sets current indent level
        /// </summary>
        public int IndentLevel {
            get => indentLevel;
            set => IndentString = SingleIndent.Repeat((uint)(indentLevel = Math.Max(0, value)));
        }

        public int MaxConsecutiveNewlines { get; init; } = 2;
    }



    public interface IFormatter<Lang> {
        public    FormatterState State { get; }
        protected int            getIndentChange<T>(Anchor<T> a) where T : Token<Lang>;
        protected bool           breakLineAfter<T>(Anchor<T> a, out int newLineCount) where T : Token<Lang>;

        protected bool whitespaceAfter<T>(Anchor<T> a) where T : Token<Lang>;

        // for now any other trivia than whitespace (i.e. preprocessor, comments) break the line at the end
        protected bool breakLineAfter<T>(Trivia<T> trivia)
            => trivia is not Whitespace<Lang> and not NewLine<Lang>; // IMPORTANT: may require anchor one day

        protected bool isFirstTokenOfStructuredTrivia<T>(Anchor<T> a) where T : Token<Lang>
            => Navigation.ParentsWithTokenOnEdge(a).Any(maybeTrivia => maybeTrivia is { Node: StructuredTrivia<Lang> });

        /**
         * Normalizing according to following rules (similar to Roslyn's)
         * - token owns all trivia immediately preceeding it up to the previous token...
         * - ...unless the trivia is last in line, then it is attached to the last token in line
         * - and the last trivia in the file is also attached to the last token
         * <remarks>good tool for checking the resulting AST of C# and basing normalized syntax on that: https://sharplab.io/</remarks>
         */
        public Token<Lang> NormalizeWhitespace<T>(Anchor<T> a) where T : Token<Lang> {
            var token = a.Node;
            var indentChange = getIndentChange(a);
            State.IndentLevel += indentChange;

            var previousToken = a.NextToken(Navigation.Side.LEFT);
            var leading = token.LeadingTriviaList;

            var breakAfterPrevious = previousToken is not null && breakLineAfter(previousToken, out var newLineCount);
            var whitespaceAfterPrevious = previousToken is not null && whitespaceAfter(previousToken);

            Trivia<Lang> startingTrivia = null;
            if ((breakAfterPrevious || leading.Any(t => t is NewLine<Lang>)) && previousToken is not null)
                startingTrivia = new NewLine<Lang>();
            else if (whitespaceAfterPrevious)
                startingTrivia = new Whitespace<Lang>();


            if (previousToken == null && isFirstTokenOfStructuredTrivia(a))
                // FIXME: this truncated leading trivia of a structured trivia syntax (it did before refactor, idk if it still does). Good for now, but generally should be fixed with a more general solution.
                leading = leading.SkipWhile(t => t is Whitespace<Lang> or NewLine<Lang>).ToList();
            else {
                var newLeading = new List<Trivia<Lang>>();
                if (previousToken == null && State.IndentLevel > 0)
                    newLeading.Add(new Whitespace<Lang>(State.IndentString));
                /*
                 * existing trivia:
                 * 1. requires nl?     -> [nl][indent][existing without starting [ws]*[nl][ws]*] -> goto 3.
                 * 2. requires ws?     -> [ws][existing without starting [ws]*[nl]*[ws]*] -> goto 3.
                 * 3. no requirements  -> [any]
                 *       -> transform each [line-breaking-trivia] into [line-breaking-trivia][nl][indent]
                 *       -> transform each [ws][ws][ws] into merged [ws]
                 *       -> transform each [nl]{maxNl+} with ([nl][indent]){maxNl}
                 */


                var consecutiveNewlines = 0; // Track consecutive newlines
                var i = 0;
                // skip first [ws]*[nl]?[ws]*
                while (i < leading.Count && leading[i] is Whitespace<Lang>) i++;
                if (i < leading.Count && leading[i] is NewLine<Lang>) i++;
                while (i < leading.Count && leading[i] is Whitespace<Lang>) i++;

                if (startingTrivia is NewLine<Lang>)
                    addNewLineAndIndent(newLeading, ref consecutiveNewlines, startingTrivia);
                else if (startingTrivia is Whitespace<Lang>)
                    newLeading.Add(startingTrivia);

                while (i < leading.Count) {
                    var trivia = leading[i];
                    if (trivia is NewLine<Lang>) {
                        consecutiveNewlines++;
                        if (consecutiveNewlines <= State.MaxConsecutiveNewlines)
                            addNewLineAndIndent(newLeading, ref consecutiveNewlines);
                    } else if (trivia is not Whitespace<Lang>) {
                        newLeading.Add(trivia);
                        consecutiveNewlines = 0;
                        if (breakLineAfter(trivia))
                            addNewLineAndIndent(newLeading, ref consecutiveNewlines);
                    }
                    i++;
                }
                leading = new TriviaList<Lang>(newLeading);
            }

            return token with { LeadingTriviaList = leading /*, TrailingTriviaList = trailing.AsTriviaList()*/ };

            void addNewLineAndIndent(List<Trivia<Lang>> triviaList, ref int consecutiveNewLines, Trivia<Lang>? nl = null) {
                triviaList.Add(nl ?? new NewLine<Lang>());
                consecutiveNewLines++;
                if (State.IndentLevel > 0)
                    triviaList.Add(new Whitespace<Lang>(State.IndentString));
            }
        }
    }
}

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
        private string SingleIndent { get; init; }        = "    ";
        public  string IndentString { get; private set; } = "";
        private int    indentLevel = 0;


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
        protected bool           breakLineAfter<T>(Anchor<T> a) where T : Token<Lang>;
        protected bool           whitespaceAfter<T>(Anchor<T> a) where T : Token<Lang>;
        // for now any other trivia than whitespace breaks the line at the end
        protected bool breakLineAfter<T>(Trivia<T> trivia) => trivia is not Whitespace<Lang>; // IMPORTANT: may require anchor one day

        private bool isFirstTokenOfStructuredTrivia<T>(Anchor<T> a) where T : Token<Lang> {
            var structuredTrivia = a.Ancestors().FirstOrDefault(p => p is { Node: StructuredTrivia<Lang> });
            if (structuredTrivia is Anchor<StructuredTrivia<Lang>> { Node: { Structure: not null } } aTrivia)
                return ReferenceEquals(a.Node, Anchor.New(aTrivia.Node.Structure, aTrivia).FirstToken()?.Node);

            return false;
        }

        private bool isLastTokenOfStructuredTrivia<T>(Anchor<T> a) where T : Token<Lang> {
            var structuredTrivia = a.Ancestors().FirstOrDefault(p => p is { Node: StructuredTrivia<Lang> });
            if (structuredTrivia is Anchor<StructuredTrivia<Lang>> { Node: { Structure: not null } } aTrivia)
                return ReferenceEquals(a.Node, Anchor.New(aTrivia.Node.Structure, aTrivia).LastToken()?.Node);

            return false;
        }

        // good tool for checking the resulting AST of C# and basing normalized syntax on that: https://sharplab.io/
        public Token<Lang> NormalizeWhitespaceOld<T>(Anchor<T> a) where T : Token<Lang> {
            var token = a.Node;
            var indentChange = getIndentChange(a);
            State.IndentLevel += indentChange;

            var previousToken = a.PreviousToken();
            var leading = token.LeadingTriviaList;

            if (previousToken is null || breakLineAfter(previousToken)) {
                if (previousToken == null && isFirstTokenOfStructuredTrivia(a)) {
                    // FIXME: this truncates leading trivia of a structured trivia syntax. Good for now, but generally should be fixed with a more general solution.
                    leading = leading.SkipWhile(t => t is Whitespace<Lang> or NewLine<Lang>).ToList();
                } else {
                    var newLeading = new List<Trivia<Lang>>();
                    if (State.IndentLevel > 0)
                        newLeading.Add(new Whitespace<Lang> { Text = State.IndentString });
                    foreach (var trivia in leading) {
                        if (breakLineAfter(trivia)) {
                            newLeading.Add(trivia);
                            if (State.IndentLevel > 0)
                                newLeading.Add(new Whitespace<Lang> { Text = State.IndentString });
                        } else if (trivia is not Whitespace<Lang> or NewLine<Lang>)
                            newLeading.Add(trivia);
                    }

                    leading = new TriviaList<Lang>(newLeading);
                }
            }

            // FIXME: existing trailing trivia (like comments and prepocessor) are lost. Refactor into something retaining important trivia.
            if (breakLineAfter(a))
                return token with { LeadingTriviaList = leading, TrailingTriviaList = new TriviaList<Lang>(new NewLine<Lang>()) };

            if (whitespaceAfter(a) && !isLastTokenOfStructuredTrivia(a))
                return token with { LeadingTriviaList = leading, TrailingTriviaList = new TriviaList<Lang>(new Whitespace<Lang>()) };

            return token with { LeadingTriviaList = leading };
        }

        public Token<Lang> NormalizeWhitespace<T>(Anchor<T> a) where T : Token<Lang> {
            var token = a.Node;
            var indentChange = getIndentChange(a);
            State.IndentLevel += indentChange;

            var previousToken = a.PreviousToken();
            var leading = token.LeadingTriviaList;

            if (previousToken is null || breakLineAfter(previousToken)) {
                if (previousToken == null && isFirstTokenOfStructuredTrivia(a))
                    // FIXME: this truncates leading trivia of a structured trivia syntax. Good for now, but generally should be fixed with a more general solution.
                    leading = leading.SkipWhile(t => t is Whitespace<Lang> or NewLine<Lang>).ToList();
                else {
                    var newLeading = new List<Trivia<Lang>>();
                    if (State.IndentLevel > 0)
                        newLeading.Add(new Whitespace<Lang> { Text = State.IndentString });

                    var consecutiveNewlines = 0; // Track consecutive newlines
                    foreach (var trivia in leading) {
                        var isNewLine = trivia is NewLine<Lang>;

                        consecutiveNewlines = isNewLine ? consecutiveNewlines + 1 : 0;

                        if (breakLineAfter(trivia)) {
                            if (!isNewLine || consecutiveNewlines <= State.MaxConsecutiveNewlines)
                                newLeading.Add(trivia);
                            if (State.IndentLevel > 0)
                                newLeading.Add(new Whitespace<Lang> { Text = State.IndentString });
                        } else {
                            newLeading.Add(trivia);
                            consecutiveNewlines = 0; // Reset consecutive newlines count
                        }
                    }

                    leading = new TriviaList<Lang>(newLeading);
                }
            }

            var trailing = token.TrailingTriviaList;

            // FIXME: existing trailing trivia (like comments and prepocessor) are lost. Refactor into something retaining important trivia.

            if (breakLineAfter(a))
                return token with { LeadingTriviaList = leading, TrailingTriviaList = new TriviaList<Lang>(new NewLine<Lang>()) };

            if (whitespaceAfter(a) && !isLastTokenOfStructuredTrivia(a))
                return token with { LeadingTriviaList = leading, TrailingTriviaList = new TriviaList<Lang>(new Whitespace<Lang>()) };

            return token with { LeadingTriviaList = leading, TrailingTriviaList = trailing };
        }
    }
}

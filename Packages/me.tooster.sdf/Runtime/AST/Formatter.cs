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
    public record FormatterState : MapperState {
        private string SingleIndent { get; init; }        = "    ";
        public  string IndentString { get; private set; } = "";
        private int    indentLevel = 0;


        /// <summary>
        /// Gets or sets current indent level
        /// </summary>
        public int IndentLevel {
            get => indentLevel;
            set {
                indentLevel = Math.Max(0, value);
                IndentString = SingleIndent.Repeat((uint)IndentLevel);
            }
        }
    }

    public interface IFormatter<Lang, out TState> where TState : FormatterState {
        public    TState State { get; }
        protected int    getIndentChange<T>(Anchor<T> a) where T : Token<Lang>;
        protected bool   breakLineAfter<T>(Anchor<T> a) where T : Token<Lang>;
        protected bool   whitespaceAfter<T>(Anchor<T> a) where T : Token<Lang>;

        public Token<Lang> NormalizeWhitespace<T>(Anchor<T> a) where T : Token<Lang> {
            var token = a.Node;
            var indentChange = getIndentChange(a);
            State.IndentLevel += indentChange;

            var previousToken = a.PreviousToken();
            TriviaList<Lang> leading = token.LeadingTriviaList;

            // FIXME: this is a bandaid, existing trivia (like comments and prepocessor) are lost. Move to something that retains important trivia
            if (previousToken is null || breakLineAfter(previousToken)) {
                // FIXME: support descending down the structured trivia such as preprocessor macros
                var insertIndentAt = leading.FindLastIndex(trivia => trivia is NewLine<Lang>);
                leading = new TriviaList<Lang>(leading.Splice(insertIndentAt + 1, 0,
                    new Whitespace<Lang> { Text = State.IndentString }));
            }

            if (breakLineAfter(a)) {
                return token with { LeadingTriviaList = leading, TrailingTriviaList = new(new NewLine<Lang>()) };
            }

            if (whitespaceAfter(a)) {
                return token with { LeadingTriviaList = leading, TrailingTriviaList = new(new Whitespace<Lang>()) };
            }

            return token with { LeadingTriviaList = leading };
        }
    }
}

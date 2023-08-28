#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AST.Shaderlab.Syntax.Trivias;
using AST.Syntax;

namespace AST.Shaderlab {
    using TriviaList = IReadOnlyList<Trivia<Shaderlab>>;


    // TODO: use SyntaxRewriter instead that returns new syntax nodes in the place of the old ones
    public class ShaderlabFormatter : SyntaxRewriter<Shaderlab> {
        public record Options(
            int  indentWidth                = 4,
            char indentCharacter            = ' ',
            int  maxConsecutiveNewlineCount = 2
        );

        private Options options;
        private int     indentLevel;
        private string  indentString; // full indent string

        private ShaderlabFormatter(Options? options = null, int indentLevel = 0) {
            this.options = options ?? new();
            this.indentLevel = indentLevel;
            indentString = new string(' ', this.options.indentWidth);
        }

        private void Indent() {
            indentLevel++;
            indentString = new string(options.indentCharacter, indentLevel * options.indentWidth);
        }

        private void Unindent() {
            indentLevel--;
            indentString = new string(options.indentCharacter, indentLevel * options.indentWidth);
        }

        public static Syntax<Shaderlab> Format(Syntax<Shaderlab> node, Options? options = null, int indentLevel = 0) {
            var formatter = new ShaderlabFormatter(options, indentLevel);
            return formatter.Visit((dynamic)node);
        }

        protected TriviaList Visit(WithParent<TriviaList> triviaListWithParent) {
            var children = base.Visit(triviaListWithParent);
            var normalized = NormalizeWhitespace(children);

            return normalized;
        }

        // non structured trivia
        protected override Trivia<Shaderlab> Visit(WithParent<Trivia<Shaderlab>> triviaWithParent) {
            var trivia = triviaWithParent.Value;
            // first leading, non-newline trivia is indent trivia - reformat it 
            if (isIndentWhitespace(triviaWithParent))
                return trivia with { Text = indentString };

            return trivia;
        }

        private static bool isIndentWhitespace(WithParent<Trivia<Shaderlab>> triviaWithParent) {
            triviaWithParent.GetType().GetProperties().First().GetSetMethod()
            var trivia = triviaWithParent.Value;
            var token = triviaWithParent.Value.Token;
            var t1 = trivia.TriviaList == token.LeadingTriviaList;
            return trivia is Whitespace
             && trivia.TriviaList.First() == token.LeadingTriviaList
                    .SkipWhile(t => t is EndOfLine)
                    .FirstOrDefault();
        }

        // We should use the red/green tree here to use dynamically built parent references to format and indent

        private IReadOnlyList<Trivia<Shaderlab>> NormalizeWhitespace(IEnumerable<Trivia<Shaderlab>> triviaList) {
            var newTriviaList = new List<Trivia<Shaderlab>>();
            var consecutiveEndOfLine = 0;
            foreach (var trivia in triviaList) {
                // remove consecutive newlines
                if (trivia is EndOfLine) ++consecutiveEndOfLine;
                else consecutiveEndOfLine = 0;

                var newTrivia = trivia switch
                {
                    Comment.Line line => throw new NotImplementedException(),
                    Comment comment => throw new NotImplementedException(),
                    EndOfLine endOfLine => consecutiveEndOfLine < options.maxConsecutiveNewlineCount ? endOfLine : null,
                    Whitespace whitespace =>,
                    StructuredTrivia<Shaderlab> structuredTrivia => throw new NotImplementedException(),
                    _ => throw new ArgumentOutOfRangeException(nameof(trivia))
                };

                if (trivia is EndOfLine) {
                    consecutiveEndOfLine++;
                    newTriviaList.Add(trivia);
                    continue;
                }

                if (trivia is Whitespace) {
                    if (lastTrivia is not Whitespace) {
                        newTriviaList.Add(trivia);
                    }
                } else {
                    newTriviaList.Add(trivia);
                }
            }

            foreach (var (previous, current) in trivia.ConsecutivePairs()) {
                var firstCurrentTrivia = current.LeadingTrivia.First();

                if (firstCurrentTrivia is not Whitespace
                 && firstCurrentTrivia is not EndOfLine
                 && previous.TrailingTrivia.Last() is not EndOfLine) {
                    var newLeading = new List<Trivia<Shaderlab>>(current.LeadingTrivia);
                    newLeading.Prepend(SyntaxFactory.Whitespace(" "));
                    previous.TrailingTrivia = new List<Trivia<Shaderlab>>();
                }
            }
        }

        private IEnumerable<Trivia<Shaderlab>> LimitConsecutiveNewlines(IEnumerable<Trivia<Shaderlab>> triviaList) {
            var consecutiveEndOfLine = 0;
            foreach (var trivia in triviaList) {
                if (trivia is EndOfLine) {
                    if (consecutiveEndOfLine >= options.maxConsecutiveNewlineCount)
                        continue;

                    ++consecutiveEndOfLine;
                    yield return trivia;
                } else consecutiveEndOfLine = 0;
            }
        }

        private IReadOnlyList<Trivia<Shaderlab>> CompresNeighborWhitespaceTrivia(
            IEnumerable<Trivia<Shaderlab>> triviaList
        ) {
            var result = new List<Trivia<Shaderlab>>();
            var lastTrivia = default(Trivia<Shaderlab>);
            foreach (var trivia in triviaList) {
                if (trivia is Whitespace && lastTrivia is Whitespace)
                    result.Add(new Whitespace { Text = lastTrivia.Text + trivia.Text });
                else
                    result.Add(trivia);

                lastTrivia = trivia;
            }

            return result;
        }

        // attaches trailing trivia of previous token (that are after the last end of line trivia) to the leading trivia of the next token
        private void NormalizeTrivia(IEnumerable<Token<Shaderlab>> tokenStream) {
            foreach (var (previous, current) in tokenStream.ConsecutivePairs()) {
                var newTrailing = new List<Trivia<Shaderlab>>();
                var newLeading = new List<Trivia<Shaderlab>>();
                var currentList = newTrailing;
                foreach (var trivia in previous.TrailingTrivia) {
                    currentList.Add(trivia);
                    if (trivia is EndOfLine) currentList = newLeading;
                }

                previous.TrailingTrivia = newTrailing;
                current.LeadingTrivia = newLeading;
            }
        }
    }
}

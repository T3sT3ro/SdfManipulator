using System;
using System.Collections.Generic;
using System.Linq;
using AST.Shaderlab.Syntax.Trivia;
using AST.Syntax;

namespace AST.Shaderlab {
    public class ShaderlabFormatter : ShaderlabSyntaxWalker {
        public record Options(
            uint indentWidth     = 4,
            char indentCharacter = ' '
        );

        private Options options;
        private uint    indentLevel;
        private string  indentString;

        private ShaderlabFormatter(Options options, uint indentLevel = 0) {
            this.options = options;
            this.indentLevel = indentLevel;
            indentString = new string(' ', (int)options.indentWidth);
        }

        public static void Format(ShaderlabSyntax node, Options options, uint indentLevel = 0) {
            var formatter = new ShaderlabFormatter(options, indentLevel);
            formatter.Visit((dynamic)node);
        }

        public override void Visit(ShaderlabSyntax node) { base.Visit(node); }

        public override void Visit(ShaderlabToken token) {
            token.LeadingTrivia = CompressWhitespace(token.LeadingTrivia);
            token.TrailingTrivia = CompressWhitespace(token.TrailingTrivia);
            base.Visit(token);
        }

        public override void Visit(ShaderlabTrivia trivia) {
            if (trivia is Whitespace) trivia.Text = " ";
            base.Visit(trivia);
        }

        private IReadOnlyList<ShaderlabTrivia> CompressWhitespace(IReadOnlyList<ShaderlabTrivia> triviaList) {
            var result = new List<ShaderlabTrivia>();
            var lastTrivia = default(ShaderlabTrivia);
            foreach (var trivia in triviaList) {
                if (trivia is Whitespace && lastTrivia is Whitespace) {
                    result.Add(new Whitespace { Text = lastTrivia.Text + trivia.Text });
                } else {
                    result.Add(trivia);
                }

                lastTrivia = trivia;
            }

            return result;
        }

        // We should use the red/green tree here to use dynamically built parent references to format and indent
        private void NormalizeWhitespace(IEnumerable<ShaderlabToken> tokenStream) {
            foreach (var (previous, current) in tokenStream.ConsecutivePairs()) {
                var firstCurrentTrivia = current.LeadingTrivia.First();

                if (firstCurrentTrivia is not Whitespace
                 && firstCurrentTrivia is not EndOfLine
                 && previous.TrailingTrivia.Last() is not EndOfLine) {
                    
                    var newLeading = new List<ShaderlabTrivia>(current.LeadingTrivia);
                    newLeading.Prepend(SyntaxFactory.Whitespace(" "));
                    previous.TrailingTrivia = new List<ShaderlabTrivia>();
                }
            }
        }

        // attaches trailing trivia of previous token (that are after the last end of line trivia) to the leading trivia of the next token
        private void NormalizeTrivia(IEnumerable<ShaderlabToken> tokenStream) {
            foreach (var (previous, current) in tokenStream.ConsecutivePairs()) {
                var newTrailing = new List<ShaderlabTrivia>();
                var newLeading = new List<ShaderlabTrivia>();
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

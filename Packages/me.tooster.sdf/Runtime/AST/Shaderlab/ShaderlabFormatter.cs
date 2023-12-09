#nullable enable
using System;
using System.Collections.Generic;
using me.tooster.sdf.AST.Shaderlab.Syntax;
using me.tooster.sdf.AST.Shaderlab.Syntax.Trivias;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab {
    public partial class  ShaderlabFormatter: Mapper<shaderlab, Formatter>, Visitor<Tree<shaderlab>.Node>, Visitor<shaderlab> {
        private ShaderlabFormatter(Formatter state) : base(state) { }

        public static T? Format<T>(T node, Formatter? s)
            where T : Tree<shaderlab>.Node {
            var formatter = new ShaderlabFormatter(s ?? new());
            return node.Accept(formatter, Anchor.New(node)) as T;
        }

        public Token<shaderlab>? Map(Token<shaderlab> token) {
            switch (token) { // FIXME: this is temporary, because existing trivia (like comments) are lost
                case OpenBraceToken: {
                    var newToken = token with { TrailingTriviaList = new(new NewLine()) };
                    state.Indent();
                    return newToken;
                }
                case CloseBraceToken:
                    state.Deindent();
                    return token with { TrailingTriviaList = new(new NewLine()) };
                default: {
                    var newToken = Visit(Anchor.New(token)) as Token<shaderlab>;
                    return newToken with { TrailingTriviaList = new(new Whitespace()) };
                }
            }
        }

        public void Visit(Anchor<SimpleTrivia<Tree<shaderlab>.Node>> a) { throw new NotImplementedException(); }

        public  SimpleTrivia<shaderlab>? Map(SimpleTrivia<shaderlab> trivia) {
            if (trivia is Whitespace)
                return trivia with { Text = " " }; // normalize whitespace lengths

            return Visit(Anchor.New(trivia)) as SimpleTrivia<shaderlab>;
        }

        public  TriviaList<shaderlab> Map(TriviaList<shaderlab> triviaList) {
            var newTriviaList = new List<Trivia<shaderlab>>();
            foreach (var trivia in triviaList) {
                if (trivia is NewLine
                 && newTriviaList[^1] is NewLine) // TODO: use the obsolete ReflectiveShaderlabFormatter logic here
                    continue;

                if (trivia is Whitespace current
                 && newTriviaList[^1] is Whitespace last) // merge consecutive whitespaces
                    newTriviaList[^1] = new Whitespace { Text = last.Text + current.Text };
                else
                    newTriviaList.Add(trivia);
            }

            return newTriviaList;
        }
    }

    /// TODO: first improve Rewriter so it returns new syntax nodes in the place of the old ones or use Mapper
    [Obsolete]
    public class ReflectiveShaderlabFormatter : Mapper<shaderlab, ReflectiveShaderlabFormatter.Options> {
        public record Options : Formatter {
            public int maxConsecutiveNewlineCount = 2;
        }

        protected ReflectiveShaderlabFormatter(Options? options = null) : base(options) { }

        public static Syntax<shaderlab> Format(Syntax<shaderlab> node, Options? options = null) {
            var formatter = new ReflectiveShaderlabFormatter(options);
            return node /*formatter.Visit((dynamic)node)*/;
        }

        /*
        protected TriviaList<shaderlab> Visit(Navigator.Navigable<TriviaList<shaderlab>> triviaListNavigable) {
            var children = base.Visit(triviaListNavigable);
            var normalized = NormalizeWhitespace(children);

            return normalized;
        }

        // non structured trivia
        protected override SimpleTrivia<shaderlab> Visit(Navigator.Navigable<SimpleTrivia<shaderlab>> triviaNavigable) {
            var trivia = triviaNavigable.Value;
            // first leading, non-newline trivia is indent trivia - reformat it
            if (isIndentWhitespace(triviaNavigable))
                return trivia with { Text = indentString };

            return trivia;
        }

        private static bool isIndentWhitespace(Navigator.Navigable<Trivia<shaderlab>> triviaNavigable) {
            triviaNavigable.GetType().GetProperties().First().GetSetMethod();
            var trivia = triviaNavigable.Value;
            var token = triviaNavigable.Value.Token;
            var t1 = trivia.TriviaList == token.LeadingTriviaList;
            return trivia is Whitespace
             && trivia.TriviaList.First() == token.LeadingTriviaList
                    .SkipWhile(t => t is EndOfLine)
                    .FirstOrDefault();
        }

        // We should use the red/green tree here to use dynamically built parent references to format and indent

        private IReadOnlyList<Trivia<shaderlab>> NormalizeWhitespace(IEnumerable<Trivia<shaderlab>> triviaList) {
            var newTriviaList = new List<Trivia<shaderlab>>();
            var consecutiveEndOfLine = 0;
            foreach (var trivia in triviaList) {
                // remove consecutive newlines
                if (trivia is EndOfLine) ++consecutiveEndOfLine;
                else consecutiveEndOfLine = 0;

                Trivia<shaderlab>? newTrivia = trivia switch
                {
                    Comment.Line line => throw new NotImplementedException(),
                    Comment comment => throw new NotImplementedException(),
                    EndOfLine endOfLine => consecutiveEndOfLine < options.maxConsecutiveNewlineCount ? endOfLine : null,
                    Whitespace whitespace => whitespace.Text == " " ? null : whitespace,
                    StructuredTrivia<shaderlab> structuredTrivia => throw new NotImplementedException(),
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
                    var newLeading = new List<Trivia<shaderlab>>(current.LeadingTrivia);
                    newLeading.Prepend(SyntaxFactory.Whitespace(" "));
                    previous.TrailingTrivia = new List<Trivia<shaderlab>>();
                }
            }
        }

        private IEnumerable<Trivia<shaderlab>> LimitConsecutiveNewlines(IEnumerable<Trivia<shaderlab>> triviaList) {
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

        private IReadOnlyList<Trivia<shaderlab>> CompresNeighborWhitespaceTrivia(
            IEnumerable<Trivia<shaderlab>> triviaList
        ) {
            var result = new List<Trivia<shaderlab>>();
            var lastTrivia = default(Trivia<shaderlab>);
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
        private void NormalizeTrivia(IEnumerable<Token<shaderlab>> tokenStream) {
            foreach (var (previous, current) in tokenStream.ConsecutivePairs()) {
                var newTrailing = new List<Trivia<shaderlab>>();
                var newLeading = new List<Trivia<shaderlab>>();
                var currentList = newTrailing;
                foreach (var trivia in previous.TrailingTrivia) {
                    currentList.Add(trivia);
                    if (trivia is EndOfLine) currentList = newLeading;
                }

                previous.TrailingTrivia = newTrailing;
                current.LeadingTrivia = newLeading;
            }
        }*/
    }
}

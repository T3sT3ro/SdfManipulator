#nullable enable
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonTrivia;
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;
using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl {
    public class HlslFormatter : Mapper<FormatterState> {
        public HlslFormatter(FormatterState state) : base(state) { }

        public static T? Format<T>(T node, FormatterState? s = null) where T : Tree<hlsl>.Node {
            var formatter = new HlslFormatter(s ?? new());
            return node.Accept(formatter, Anchor.New(node)) as T;
        }

        private static int getIndentChange<T>(Anchor<T> a) where T : Token<hlsl> => a switch
        {
            {
                Node: OpenBraceToken
            } => +1,
            {
                Node: CloseBraceToken
            } => -1,
            _ => 0
        };

        private static bool breakLineAfter<T>(Anchor<T> a) where T : Token<hlsl> {
            switch (a) {
                case { Node: OpenBraceToken or CloseBraceToken or SemicolonToken }:
                    return true;
                default:
                    return false;
            }
        }

        public override Tree<hlsl>.Node? Visit(Anchor<Token<hlsl>> a) {
            var token = base.Visit(a) as Token<hlsl>;

            var indentChange = getIndentChange(a);
            if (indentChange < 0) state.CurrentIndentLevel--;
            if (indentChange > 0) state.CurrentIndentLevel++;

            bool isFirstInLine = state.PollLineStart(out var startingIndent);
            TriviaList<hlsl>? leading = token.LeadingTriviaList;

            if (startingIndent is not null) {
                leading = new TriviaList<hlsl>(new Whitespace<hlsl> { Text = startingIndent });
            }

            if (breakLineAfter(a)) {
                state.MarkLineEnd();
                return token with { LeadingTriviaList = leading, TrailingTriviaList = new(new NewLine<hlsl>()) };
            }

            return token with { LeadingTriviaList = leading, TrailingTriviaList = new(new Whitespace<hlsl>()) };
        }
    }
}

#nullable enable
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonTrivia;
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;
using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl {
    public class HlslFormatter : Mapper<FormatterState>, IFormatter<hlsl, FormatterState> {
        public HlslFormatter(FormatterState state) : base(state) { }

        public static T? Format<T>(T node, FormatterState? s = null) where T : Tree<hlsl>.Node {
            var formatter = new HlslFormatter(s ?? new());
            return node.Accept(formatter, Anchor.New(node)) as T;
        }

        private static int getIndentChange<T>(Anchor<T> a) where T : Token<hlsl> => a switch
        {
            { Node: OpenBraceToken }  => +1,
            { Node: CloseBraceToken } => -1,
            _                         => 0
        };

        private static bool breakLineAfter<T>(Anchor<T> a) where T : Token<hlsl> {
            return a is { Node: OpenBraceToken or CloseBraceToken or SemicolonToken };
        }

        private static bool whitespaceAfter<T>(Anchor<T> a) where T : Token<hlsl> {
            return a.NextToken() is not {Node: OpenParenToken or CloseParenToken or SemicolonToken };
        }

        public override Tree<hlsl>.Node? Visit(Anchor<Token<hlsl>> a) {
            if (base.Visit(a) is not Token<hlsl> token) return null;

            return ((IFormatter<hlsl, FormatterState>)this).NormalizeWhitespace(Anchor.New(token, a.Parent));
        }

        public FormatterState                 State                           => state;
        int IFormatter<hlsl, FormatterState>. getIndentChange<T>(Anchor<T> a) => getIndentChange(a);
        bool IFormatter<hlsl, FormatterState>.breakLineAfter<T>(Anchor<T> a)  => breakLineAfter(a);
        bool IFormatter<hlsl, FormatterState>.whitespaceAfter<T>(Anchor<T> a) => whitespaceAfter(a);
    }
}

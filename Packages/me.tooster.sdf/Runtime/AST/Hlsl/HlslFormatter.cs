#nullable enable
using System.Linq;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonTrivia;
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;
using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl {
    // For reference of formatting HLSL check out:
    //  https://github.com/tgjones/HlslTools/blob/master/src/ShaderTools.CodeAnalysis.Hlsl/Formatting/FormattingVisitor.cs
    public class HlslFormatter : Mapper, IFormatter<hlsl> {
        public FormatterState State { get; }

        public HlslFormatter(FormatterState? state = null) {
            descendIntoTrivia = true;
            State = state ?? new FormatterState();
        }

        public static T? Format<T>(T node, FormatterState? state = null) where T : Tree<hlsl>.Node {
            var formatter = new HlslFormatter(state);
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
            if (a is { Node: HashToken, Parent: { Node: PreprocessorSyntax } }) return false;

            var nextToken = a.NextToken();
            if(nextToken is {Node: OpenParenToken or CloseParenToken or SemicolonToken }) return false;

            return true;
        }

        public override Tree<hlsl>.Node? Visit(Anchor<Token<hlsl>> a) {
            if (base.Visit(a) is not Token<hlsl> token) return null;

            return ((IFormatter<hlsl>)this).NormalizeWhitespace(Anchor.New(token, a.Parent));
        }

        int IFormatter<hlsl>. getIndentChange<T>(Anchor<T> a) => getIndentChange(a);
        bool IFormatter<hlsl>.breakLineAfter<T>(Anchor<T> a)  => breakLineAfter(a);
        bool IFormatter<hlsl>.whitespaceAfter<T>(Anchor<T> a) => whitespaceAfter(a);
    }
}

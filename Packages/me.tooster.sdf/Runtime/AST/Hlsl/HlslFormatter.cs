#nullable enable
using System.Linq;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonTrivia;
using FunctionDefinition = me.tooster.sdf.AST.Hlsl.Syntax.Statements.FunctionDefinition;

namespace me.tooster.sdf.AST.Hlsl {
    // For reference of formatting HLSL check out:
    //  https://github.com/tgjones/HlslTools/blob/master/src/ShaderTools.CodeAnalysis.Hlsl/Formatting/FormattingVisitor.cs
    public class HlslFormatter : Mapper, IFormatter<hlsl> {
        public FormatterState State { get; }

        public HlslFormatter(FormatterState? state = null) {
            descendIntoTrivia = true;
            State = state ?? new FormatterState();
        }

        public static T? Format<T>(T? node, FormatterState? state = null) where T : Tree<hlsl>.Node {
            if (node is null) return null;
            var formatter = new HlslFormatter(state);
            return node.Accept(formatter, Anchor.New(node)) as T;
        }

        static int getIndentChange<T>(Anchor<T> a) where T : Token<hlsl>
            => a switch
            {
                { Node: OpenBraceToken or OpenParenToken or OpenBracketToken }  => +1,
                { Node: CloseBraceToken or CloseParenToken or CloseBraceToken } => -1,
                _                                                               => 0,
            };

        static bool breakLineAfter<T>(Anchor<T> a, out int newLineCount) where T : Token<hlsl> {
            newLineCount = 1;
            if (a is { Parent: { Node: IBracedList, Parent: { Node: not Block } }, Node: OpenBraceToken or CloseBraceToken })
                return false;

            if (a is { Node: SemicolonToken or CloseBraceToken or OpenBraceToken })
                return true;

            var nextToken = a.NextToken();
            switch (nextToken) {
                case { Node: EndOfDirectiveToken }: return false;
                case { Node: CloseBraceToken }:     return nextToken is not { Parent: { Node: IBracedList } };
                case { Node: { } t } when Navigation.ParentsWithTokenOnEdge(nextToken).Any(a => a is { Node: FunctionDefinition }):
                    return true;
                default: return false;
            }
        }

        static bool whitespaceAfter<T>(Anchor<T> a) where T : Token<hlsl> {
            switch (a) {
                case { Node  : OpenParenToken or OpenBracketToken or ColonColonToken }
                    or { Node: HashToken, Parent: { Node: PreprocessorSyntax } }:
                case { Node: DotToken, Parent: { Node: Member } }:
                    return false;
            }

            if (a.Parent is { Node: Unary u } && ReferenceEquals(a.Node, u.operatorToken))
                return false;

            var nextToken = a.NextToken();
            switch (nextToken) {
                case { Node: CloseParenToken or SemicolonToken or ColonColonToken or CommaToken or EndOfDirectiveToken }:
                case { Node: DotToken, Parent: { Node: Member } }:
                case { Node: OpenParenToken op, Parent: { Node: IArgumentList argList, Parent: { Node: Call or FunctionDefinition } } }
                    when ReferenceEquals(argList.openParenToken, op):

                    return false;
            }

            return true;
        }

        public override Tree<hlsl>.Node? Visit(Anchor<Token<hlsl>> a) {
            if (base.Visit(a) is not Token<hlsl> token) return null;

            var fmt = (IFormatter<hlsl>)this;

            // empty line before function, unless it's the first statement of enclosing structure
            if (a.Parent is Anchor<SyntaxOrToken<hlsl>> { Node: FunctionDefinition } sa && ReferenceEquals(token, sa.FirstToken()?.Node)
             && a.NextToken(Navigation.Side.LEFT) is not { Node: OpenBraceToken })
                a = Anchor.New(token with { LeadingTriviaList = token.LeadingTriviaList.Splice(0, 0, new NewLine<hlsl>()) }, a.Parent);

            return fmt.NormalizeWhitespace(Anchor.New(token, a.Parent));
        }

        int IFormatter<hlsl>. getIndentChange<T>(Anchor<T> a)                      => getIndentChange(a);
        bool IFormatter<hlsl>.breakLineAfter<T>(Anchor<T> a, out int newLineCount) => breakLineAfter(a, out newLineCount);
        bool IFormatter<hlsl>.whitespaceAfter<T>(Anchor<T> a)                      => whitespaceAfter(a);
    }
}

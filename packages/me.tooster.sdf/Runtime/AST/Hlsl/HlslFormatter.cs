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
        public HlslFormatter(FormatterState? state = null) {
            descendIntoTrivia = true;
            State = state ?? new FormatterState();
        }
        public FormatterState State { get; }

        int IFormatter<hlsl>. getIndentChange<T>(Anchor<T> a)                      => getIndentChange(a);
        bool IFormatter<hlsl>.breakLineAfter<T>(Anchor<T> a, out int newLineCount) => breakLineAfter(a, out newLineCount);
        bool IFormatter<hlsl>.whitespaceAfter<T>(Anchor<T> a)                      => whitespaceAfter(a);

        public static T? Format<T>(T? node, FormatterState? state = null) where T : Tree<hlsl>.Node {
            if (node is null)
                return null;
            var formatter = new HlslFormatter(state);
            return node.Accept(formatter, null) as T;
        }

        static int getIndentChange<T>(Anchor<T> a) where T : Token<hlsl>
            => a switch
            {
                // a case label indents its statements up to the next label, or up to the end of the switch
                { Node: ColonToken, Parent: { Node: Switch.Case or Switch.DefaultCase } } => +1,
                { Node: CaseKeyword or DefaultKeyword }
                    when a.NextToken(Navigation.Side.LEFT) is not { Node: OpenBraceToken, Parent: { Node: Switch } } => -1,
                { Node: CloseBraceToken, Parent: { Node: Switch s } } => s.cases.Count > 0 || s.defaultCase is not null ? -2 : -1,

                { Node: OpenBraceToken or OpenParenToken or OpenBracketToken }    => +1,
                { Node: CloseBraceToken or CloseParenToken or CloseBracketToken } => -1,
                _                                                                 => 0,
            };

        static bool breakLineAfter<T>(Anchor<T> a, out int newLineCount) where T : Token<hlsl> {
            newLineCount = 1;
            if (a is { Parent: { Node: IBracedList, Parent: { Node: not Block } }, Node: OpenBraceToken or CloseBraceToken })
                return false;

            var nextToken = a.NextToken();

            // a blank line before a function definition, unless it's the first statement of a block
            if (a is not { Node: OpenBraceToken } && nextToken is not null
             && Navigation.ParentsWithTokenOnEdge(nextToken).Any(p => p is { Node: FunctionDefinition })) {
                newLineCount = 2;
                return true;
            }

            switch (a) {
                case { Node: ColonToken, Parent: { Node: Switch.Case or Switch.DefaultCase } }: return true;
                // for (int i = 0; i < 4; i++)
                case { Node: SemicolonToken, Parent: { Node: For } }: return false;
                // "} else", "} while (x);" and "};"
                case { Node: CloseBraceToken } when nextToken is { Node: ElseKeyword or SemicolonToken }
                                                              or { Node: WhileKeyword, Parent: { Node: DoWhile } }: return false;
                case { Node: SemicolonToken or CloseBraceToken or OpenBraceToken }: return true;
            }

            return nextToken switch
            {
                { Node: EndOfDirectiveToken } => false,
                { Node: CloseBraceToken }     => nextToken is not { Parent: { Node: IBracedList } },
                _                             => false,
            };
        }

        static bool whitespaceAfter<T>(Anchor<T> a) where T : Token<hlsl> {
            switch (a) {
                case { Node  : OpenParenToken or OpenBracketToken or ColonColonToken }
                    or { Node: HashToken, Parent: { Node: PreprocessorSyntax } }:
                case { Node: DotToken, Parent: { Node: Member } }:
                // ++i
                case { Node: AffixOperatorToken, Parent: { Node: Affix.Pre } }: return false;
            }

            var nextToken = a.NextToken();

            // "-x", but "- -x" and "- -1.0", which would otherwise read as the decrement "--"
            if (a.Parent is { Node: Unary u } && ReferenceEquals(a.Node, u.operatorToken))
                return a.Node.Text is "-" or "+" && nextToken is not null && nextToken.Node.Text.StartsWith(a.Node.Text);

            switch (nextToken) {
                case { Node: CloseParenToken or SemicolonToken or ColonColonToken or CommaToken or EndOfDirectiveToken }:
                case { Node: DotToken, Parent: { Node: Member } }:
                // i++, a[0], float a[4] and "case 1:"
                case { Node: AffixOperatorToken, Parent: { Node: Affix.Post } }:
                case { Node: CloseBracketToken }:
                case { Node: OpenBracketToken, Parent: { Node: Indexer or ArrayRank } }:
                case { Node: ColonToken, Parent: { Node: Switch.Case or Switch.DefaultCase } }:
                // a function-like macro needs the parenthesis right after its name, or it becomes an object-like macro
                case { Node: OpenParenToken op, Parent: { Node: IArgumentList argList, Parent: { Node: Call or FunctionDefinition or FunctionDeclaration or Define } } }
                    when ReferenceEquals(argList.openParenToken, op): return false;
            }

            return true;
        }

        public override Tree<hlsl>.Node? Visit(Anchor<Token<hlsl>> a) {
            if (base.Visit(a) is not Token<hlsl> token)
                return null;

            // navigate with the original anchor: after formatting its trivia, the token is a new instance outside the tree
            return ((IFormatter<hlsl>)this).NormalizeWhitespace(a, token);
        }
    }
}

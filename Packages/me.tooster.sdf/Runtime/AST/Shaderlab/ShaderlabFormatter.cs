#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Shaderlab.Syntax;
using me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific;
using me.tooster.sdf.AST.Shaderlab.Syntax.SubShaderSpecific;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using Whitespace = me.tooster.sdf.AST.Syntax.CommonTrivia.Whitespace<me.tooster.sdf.AST.shaderlab>;
using NewLine = me.tooster.sdf.AST.Syntax.CommonTrivia.NewLine<me.tooster.sdf.AST.shaderlab>;

namespace me.tooster.sdf.AST.Shaderlab {
    public class ShaderlabFormatter : Mapper<FormatterState>, IFormatter<shaderlab, FormatterState> {
        private ShaderlabFormatter(FormatterState state) : base(state) { }

        public FormatterState                      State                           => state;
        int IFormatter<shaderlab, FormatterState>. getIndentChange<T>(Anchor<T> a) => getIndentChange(a);
        bool IFormatter<shaderlab, FormatterState>.breakLineAfter<T>(Anchor<T> a)  => breakLineAfter(a);
        bool IFormatter<shaderlab, FormatterState>.whitespaceAfter<T>(Anchor<T> a) => whitespaceAfter(a);

        // Normalizes the syntax, e.g. whitespaces, newlines etc.
        public static T? Format<T>(T node, FormatterState? s = null) where T : Tree<shaderlab>.Node {
            var formatter = new ShaderlabFormatter(s ?? new());
            return node.Accept(formatter, Anchor.New(node)) as T;
        }

        protected int getIndentChange<T>(Anchor<T> a) where T : Token<shaderlab> => a switch
        {
            {
                Node: OpenBraceToken, Parent: { Node: MaterialProperties or SubShader or Pass or TagsBlock or Shader }
            } => +1,
            {
                Node: CloseBraceToken, Parent: { Node: MaterialProperties or SubShader or Pass or TagsBlock or Shader }
            } => -1,
            _ => 0
        };

        private static bool breakLineAfter<T>(Anchor<T> a) where T : Token<shaderlab> {
            if (a is {
                    Node: OpenBraceToken or CloseBraceToken or HlslProgramKeyword or EndHlslKeyword
                    or HlslIncludeKeyword
                }
                or { Node: QuotedStringLiteral, Parent: Anchor<CommandArgument> })
                return true;

            var nextToken = a.NextToken();
            if (nextToken is { Node: CloseBraceToken })
                return true;

            return a.Ancestors().Any(parent =>
                    parent is IAnchor<SyntaxOrToken<shaderlab>> { Node: Command or Property or Tag } aSyntax
                 && aSyntax.LastToken() == a.Node);
        }

        private static bool whitespaceAfter<T>(Anchor<T> a) where T : Token<shaderlab> {
            if (a is { Node: OpenBracketToken or OpenParenToken or DotToken }) return false;

            return a.NextToken() is not { Node: CloseBracketToken or CloseBraceToken };
        }

        public override Tree<shaderlab>.Node? Visit(Anchor<Token<shaderlab>> a) {
            if (base.Visit(a) is not Token<shaderlab> token) return null;

            return ((IFormatter<shaderlab, FormatterState>)this).NormalizeWhitespace(Anchor.New(token, a.Parent));
        }

        public override Tree<shaderlab>.Node? Visit(Anchor<SimpleTrivia<shaderlab>> a) {
            var trivia = a.Node;
            if (trivia is Whitespace)
                return trivia with { Text = " " }; // normalize whitespace lengths

            return base.Visit(Anchor.New(trivia));
        }

        public override Tree<shaderlab>.Node? Visit(Anchor<TriviaList<shaderlab>> a) {
            var newTriviaList = new List<Trivia<shaderlab>>();
            var changed = false;
            foreach (var trivia in a.Node) {
                switch (trivia) {
                    // TODO: use the obsolete ReflectiveShaderlabFormatter logic here
                    // TODO: visit children trivia when needed
                    case NewLine when newTriviaList[^1] is NewLine: continue;
                    // merge consecutive whitespaces
                    case Whitespace current when newTriviaList[^1] is Whitespace last:
                        newTriviaList[^1] = new Whitespace { Text = last.Text + current.Text };
                        changed = true;
                        break;
                    default:
                        if (base.Visit(Anchor.New(trivia)) is Trivia<shaderlab> newTrivia) {
                            newTriviaList.Add(newTrivia);
                            changed = true;
                        }

                        break;
                }
            }

            return changed ? new TriviaList<shaderlab>(newTriviaList) : a;
        }

        public override Tree<shaderlab>.Node? Visit<T>(Anchor<InjectedLanguage<shaderlab, T>> a) => a switch
        {
            { Node: InjectedLanguage<shaderlab, hlsl> injected } => new InjectedLanguage<shaderlab, hlsl>(
                new Tree<hlsl>(HlslFormatter.Format(
                    injected.tree?.Root,
                    new FormatterState { IndentLevel = state.IndentLevel }))),
            _ => a.Node
        };
    }
}

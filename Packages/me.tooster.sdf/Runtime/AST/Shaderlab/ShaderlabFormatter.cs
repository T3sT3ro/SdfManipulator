#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Shaderlab.Syntax;
using me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific;
using me.tooster.sdf.AST.Shaderlab.Syntax.SubShaderSpecific;
using me.tooster.sdf.AST.Shaderlab.Syntax.Trivias;
using me.tooster.sdf.AST.Syntax;
using CloseBraceToken = me.tooster.sdf.AST.Shaderlab.Syntax.CloseBraceToken;
using OpenBraceToken = me.tooster.sdf.AST.Shaderlab.Syntax.OpenBraceToken;

namespace me.tooster.sdf.AST.Shaderlab {
    public class ShaderlabFormatter : Mapper<FormatterState> {
        private ShaderlabFormatter(FormatterState state) : base(state with { }) { }

        public static T? Format<T>(T node, FormatterState? s = null)
            where T : Tree<shaderlab>.Node {
            var formatter = new ShaderlabFormatter(s ?? new());
            return node.Accept(formatter, Anchor.New(node)) as T;
        }

        private static int getIndentChange<T>(Anchor<T> a) where T : Token<shaderlab> => a switch
        {
            { Node: OpenBraceToken, Parent : { Node: MaterialProperties or SubShader or Pass or TagsBlock or Shader } } => +1,
            { Node: CloseBraceToken, Parent: { Node: MaterialProperties or SubShader or Pass or TagsBlock or Shader} } => -1,
            _                                                                                                 => 0
        };

        private static bool breakLineAfter<T>(Anchor<T> a) where T : Token<shaderlab> {
            switch (a) {
                case {
                        Node: OpenBraceToken or CloseBraceToken or HlslProgramKeyword or EndHlslKeyword
                        or HlslIncludeKeyword
                    }
                    or { Node: QuotedStringLiteral, Parent: Anchor<CommandArgument> }:
                    return true;
                default:
                    var nextToken = Navigation.getNextToken<shaderlab, Token<shaderlab>>(a);
                    if (nextToken is CloseBraceToken)
                        return true;

                    return Navigation.Ancestors(a).Any(parent =>
                        parent is { Node: Command } or { Node: Property } or { Node: Tag }
                     && Navigation.getLastToken<shaderlab, Syntax<shaderlab>>((IAnchor<Syntax<shaderlab>>)parent)
                     == a.Node);
            }
        }

        public override Tree<shaderlab>.Node? Visit(Anchor<Token<shaderlab>> a) {
            var token = a.Node;

            var indentChange = getIndentChange(a);
            if (indentChange < 0) state.Deindent();
            if (indentChange > 0) state.Indent();

            bool isFirstInLine = state.PollLineStart(out var startingIndent);
            TriviaList<shaderlab>? leading = token.LeadingTriviaList;

            if (startingIndent is not null) {
                leading = new TriviaList<shaderlab>(new Whitespace { Text = startingIndent });
            }

            // FIXME: this is a bandaid, because existing trivia (like comments) are lost. Move to something that retains important trivia
            if (breakLineAfter(a)) {
                state.MarkLineEnd();
                return token with { LeadingTriviaList = leading, TrailingTriviaList = new(new NewLine()) };
            }

            return token with { LeadingTriviaList = leading, TrailingTriviaList = new(new Whitespace()) };
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
    }
}

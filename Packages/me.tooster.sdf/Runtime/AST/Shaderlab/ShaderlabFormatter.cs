#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Shaderlab.Syntax;
using me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific;
using me.tooster.sdf.AST.Shaderlab.Syntax.SubShaderSpecific;
using me.tooster.sdf.AST.Shaderlab.Syntax.Trivias;
using me.tooster.sdf.AST.Syntax;
using CloseBraceToken = me.tooster.sdf.AST.Shaderlab.Syntax.CloseBraceToken;
using OpenBraceToken = me.tooster.sdf.AST.Shaderlab.Syntax.OpenBraceToken;

namespace me.tooster.sdf.AST.Shaderlab {
    public class ShaderlabFormatter : Mapper<FormatterState> {
        private ShaderlabFormatter(FormatterState state) : base(state) { }

        public static T? Format<T>(T node, FormatterState? s = null) where T : Tree<shaderlab>.Node {
            var formatter = new ShaderlabFormatter(s ?? new());
            return node.Accept(formatter, Anchor.New(node)) as T;
        }

        private static int getIndentChange<T>(Anchor<T> a) where T : Token<shaderlab> => a switch
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
                     && Navigation.getLastToken((IAnchor<Syntax<shaderlab>>)parent)
                     == a.Node);
            }
        }

        public override Tree<shaderlab>.Node? Visit(Anchor<Token<shaderlab>> a) {
            if (base.Visit(a) is not Token<shaderlab> token) return null;

            var indentChange = getIndentChange(a);
            if (indentChange < 0) state.CurrentIndentLevel--;
            if (indentChange > 0) state.CurrentIndentLevel++;

            bool isFirstInLine = state.PollLineStart(out var startingIndent);
            TriviaList<shaderlab> leading = token.LeadingTriviaList;

            // FIXME: this is a bandaid, existing trivia (like comments and prepocessor) are lost. Move to something that retains important trivia
            if (startingIndent is not null) {
                // FIXME: support descending down the structured trivia such as preprocessor macros
                var insertIndentAt = leading.FindLastIndex(trivia => trivia is NewLine);
                leading = new TriviaList<shaderlab>(leading.Splice(insertIndentAt + 1, 0,
                    new Whitespace { Text = startingIndent }));
            }

            if (!breakLineAfter(a))
                return token with { LeadingTriviaList = leading, TrailingTriviaList = new(new Whitespace()) };

            state.MarkLineEnd();
            return token with { LeadingTriviaList = leading, TrailingTriviaList = new(new NewLine()) };
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
                    new FormatterState { CurrentIndentLevel = state.CurrentIndentLevel }))),
            _ => a.Node
        };
    }
}

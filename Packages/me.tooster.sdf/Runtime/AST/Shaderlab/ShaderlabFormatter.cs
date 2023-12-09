#nullable enable
using System;
using System.Collections.Generic;
using me.tooster.sdf.AST.Shaderlab.Syntax;
using me.tooster.sdf.AST.Shaderlab.Syntax.Trivias;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab {
    public class ShaderlabFormatter : Mapper<FormatterState> {
        private ShaderlabFormatter(FormatterState state) : base(state) { }

        public static T? Format<T>(T node, FormatterState? s = null)
            where T : Tree<shaderlab>.Node {
            var formatter = new ShaderlabFormatter(s ?? new());
            return node.Accept((Visitor<shaderlab, T?>)formatter, Anchor.New(node)) as T;
        }

        public override Tree<shaderlab>.Node? Visit(Anchor<Token<shaderlab>> a) {
            var token = a.Node;
            switch (token) {
                // FIXME: this is temporary, because existing trivia (like comments) are lost. Make something that retains comments etc.
                case OpenBraceToken: {
                    var newToken = token with { TrailingTriviaList = new(new NewLine()) };
                    state.Indent();
                    return newToken;
                }
                case CloseBraceToken:
                    state.Deindent();
                    return token with { TrailingTriviaList = new(new NewLine()) };
                default: {
                    // TODO keep trivia
                    return token with { TrailingTriviaList = new(new Whitespace()) };
                }
            }
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

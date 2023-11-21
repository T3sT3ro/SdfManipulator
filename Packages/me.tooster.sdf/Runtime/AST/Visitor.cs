#nullable enable
using System;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST {
    // Visitor returning result for visited nodes
    public abstract class Visitor<Lang, TResult> {
        public virtual TResult? Visit(Object? @default) { return default; }

        public virtual TResult? Visit(Syntax<Lang>? syntax) { return default; }

        public virtual TResult? Visit(Token<Lang>? token) { return default; }

        public virtual TResult? Visit(TriviaList<Lang>? triviaList) { return default; }

        public virtual TResult? Visit(Trivia<Lang>? trivia) { return default; }
    }

    // Visitor without return values
    public abstract class Visitor<Lang> {
        public virtual void Visit(Object? @default) { }

        public virtual void Visit(Syntax<Lang>? syntax) { }

        public virtual void Visit(Token<Lang>? token) { }

        public virtual void Visit(TriviaList<Lang>? triviaList) { }

        public virtual void Visit(Trivia<Lang>? trivia) { }
    }
}

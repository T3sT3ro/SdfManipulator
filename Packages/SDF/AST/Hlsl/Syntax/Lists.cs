using System;
using System.Collections.Generic;
using System.Linq;
using AST.Hlsl.Syntax.Expressions;
using AST.Syntax;

namespace AST.Hlsl.Syntax {
    // ( T , ... , T ) 
    public record ArgumentList<T> : Syntax<Hlsl> where T : Syntax<Hlsl> {
        public OpenParenToken         openParenToken  { get; init; } = new();
        public SeparatedList<Hlsl, T> arguments       { get; init; } = SeparatedList<Hlsl, T>.Empty;
        public CloseParenToken        closeParenToken { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { openParenToken, arguments, closeParenToken };

        public static implicit operator ArgumentList<T>(T[] args) => new() { arguments = args.CommaSeparated() };

        public static implicit operator ArgumentList<T>(T singleton) =>
            new() { arguments = singleton.CommaSeparated() };
    }

    // { T , ... , T }
    public record BracedList<T> : Syntax<Hlsl> where T : Syntax<Hlsl> {
        public OpenBraceToken         openBraceToken  { get; init; } = new();
        public SeparatedList<Hlsl, T> arguments       { get; init; } = SeparatedList<Hlsl, T>.Empty;
        public CloseBraceToken        closeBraceToken { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { openBraceToken, arguments, closeBraceToken };

        public static implicit operator BracedList<T>(T[] args) => new() { arguments = args.CommaSeparated() };
    }
}

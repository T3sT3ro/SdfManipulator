using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    // ( T , ... , T ) 
    [AstSyntax] public partial record ArgumentList<T> : Syntax<Hlsl> where T : Syntax<Hlsl> {
        public OpenParenToken         openParenToken { get; init; } = new();
        public SeparatedList<Hlsl, T> arguments { get; init; } = new();
        public CloseParenToken        closeParenToken { get; init; } = new();

        public static implicit operator ArgumentList<T>(T[] args) => new() { arguments = args.CommaSeparated() };

        public static implicit operator ArgumentList<T>(T singleton) =>
            new() { arguments = singleton.CommaSeparated() };
    }

    // { T , ... , T }
    [AstSyntax] public partial record BracedList<T> : Syntax<Hlsl> where T : Syntax<Hlsl> {
        public OpenBraceToken         openBraceToken { get; init; } = new();
        public SeparatedList<Hlsl, T> arguments { get; init; } = new();
        public CloseBraceToken        closeBraceToken { get; init; } = new();

        public static implicit operator BracedList<T>(T[] args)    => new() { arguments = args.CommaSeparated() };
        public static implicit operator BracedList<T>(T singleton) => new() { arguments = singleton.CommaSeparated() };
    }
}

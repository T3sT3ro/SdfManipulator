using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    // ( T , ... , T ) 
    [SyntaxNode] public partial record ArgumentList<T> : Syntax<hlsl> where T : Syntax<hlsl> {
        public OpenParenToken         openParenToken  { get; init; } = new();
        public SeparatedList<hlsl, T> arguments       { get; init; } = new();
        public CloseParenToken        closeParenToken { get; init; } = new();

        public static implicit operator ArgumentList<T>(T[] args) => new() { arguments = args.CommaSeparated() };

        public static implicit operator ArgumentList<T>(T singleton) =>
            new() { arguments = singleton.CommaSeparated() };
    }

    // { T , ... , T }
    [SyntaxNode] public partial record BracedList<T> : Syntax<hlsl> where T : Syntax<hlsl> {
        public OpenBraceToken         openBraceToken  { get; init; } = new();
        public SeparatedList<hlsl, T> arguments       { get; init; } = new();
        public CloseBraceToken        closeBraceToken { get; init; } = new();

        public static implicit operator BracedList<T>(T[] args)    => new() { arguments = args.CommaSeparated() };
        public static implicit operator BracedList<T>(T singleton) => new() { arguments = singleton.CommaSeparated() };
    }
}

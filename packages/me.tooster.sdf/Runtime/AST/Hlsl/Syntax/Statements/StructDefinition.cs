#nullable enable
using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    // struct x {};
    // struct x { float pos; x other; };
    // struct {}; anonymous, empty, allowed but... pointless?
    [SyntaxNode] public partial record StructDefinition : Statement {
        public Type.Struct    shape     { get; init; }
        public SemicolonToken semicolon { get; init; } = new();

        public static implicit operator StructDefinition(Type.Struct shape) => new() { shape = shape };
    }
}

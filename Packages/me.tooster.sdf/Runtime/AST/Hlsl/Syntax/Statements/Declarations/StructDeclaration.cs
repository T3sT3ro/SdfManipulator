#nullable enable
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;
using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements.Declarations {
    // struct x {};
    // struct x { float pos; x other; };
    // struct {}; anonymous, empty, allowed but... pointless?
    [SyntaxNode] public partial record StructDeclaration : Statement {
        public Type.Struct    shape     { get; init; }
        public SemicolonToken semicolon { get; init; } = new();

        public static implicit operator StructDeclaration(Type.Struct shape) => new() { shape = shape };
    }
}

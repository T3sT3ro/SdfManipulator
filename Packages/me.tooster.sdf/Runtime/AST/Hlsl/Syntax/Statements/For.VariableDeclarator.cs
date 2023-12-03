#nullable enable
namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record For {
        [SyntaxNode] public partial record VariableDeclarator : Initializer {
            public VariableDeclarator declarator { get; internal init; }
        }
    }
}

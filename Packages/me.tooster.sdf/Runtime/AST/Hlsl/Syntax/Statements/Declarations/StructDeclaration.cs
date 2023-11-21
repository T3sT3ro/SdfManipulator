#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

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

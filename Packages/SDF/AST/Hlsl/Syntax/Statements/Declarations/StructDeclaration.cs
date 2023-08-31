#nullable enable
using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements.Declarations {
    // struct x {};
    // struct x { float pos; x other; };
    // struct {}; anonymous, empty, allowed but... pointless?
    public partial record StructDeclaration : Statement {
        private readonly Type.Struct _shape;
        private readonly SemiToken   _semicolon;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { shape, semicolon };

        public static implicit operator StructDeclaration(Type.Struct shape) => new() { shape = shape };
    }
}

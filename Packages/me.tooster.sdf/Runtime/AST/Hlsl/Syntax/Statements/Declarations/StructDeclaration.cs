#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements.Declarations {
    // struct x {};
    // struct x { float pos; x other; };
    // struct {}; anonymous, empty, allowed but... pointless?
    public partial record StructDeclaration : Statement {
        private readonly Type.Struct /*_*/shape;
        private readonly SemiToken                    /*_*/semicolon;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { shape, semicolon };

        public static implicit operator StructDeclaration(Type.Struct shape) => new() { shape = shape };
    }
}

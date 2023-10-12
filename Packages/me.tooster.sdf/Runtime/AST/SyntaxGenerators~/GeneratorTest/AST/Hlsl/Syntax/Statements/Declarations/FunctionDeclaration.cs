#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements.Declarations {
    // int foo(float x, row_major float y : VPOS = 7.0f) { return x + y; }
    [AstSyntax] public partial record FunctionDeclaration : Statement {
        public        Type                    returnType { get; init; }
        public        Identifier              id { get; init; }
        public ArgumentList<Parameter> paramList { get; init; } = new();
        public        Semantic?               returnSemantic { get; init; }
        public        Block                   body { get; init; }

        /// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-function-parameters">HLSL parameter syntax</a>
        [AstSyntax] public partial record Parameter : Syntax<Hlsl> {
            public Token<Hlsl>? modifier { get; init; }
            public Type         type { get; init; }
            public Identifier   id { get; init; }
            public Semantic?    semantic { get; init; }
            public Expression?  initializer { get; init; }
        }
    }
}
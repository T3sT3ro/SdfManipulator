#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements.Declarations {
    // int foo(float x, row_major float y : VPOS = 7.0f) { return x + y; }
    public record FunctionDeclaration : Statement {
        public Type                    returnType     { get; init; }
        public Identifier              id             { get; init; }
        public ArgumentList<Parameter> paramList      { get; init; }
        public Semantic?               returnSemantic { get; init; }
        public Block                   body           { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new Syntax<Hlsl>?[]
                { returnType, id, paramList, returnSemantic, body }
            .FilterNotNull().ToList();

        /// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-function-parameters">HLSL parameter syntax</a>
        public record Parameter : Syntax<Hlsl> {
            public Token<Hlsl>? modifier    { get; init; }
            public Type         type        { get; init; }
            public Identifier   id          { get; init; }
            public Semantic?    semantic    { get; init; }
            public Expression?  initializer { get; init; }

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
                    { modifier, type, id, semantic, initializer }
                .FilterNotNull().ToList();
        }
    }
}

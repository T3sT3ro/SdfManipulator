#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Hlsl.Syntax.Expressions;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements.Declarations {
    // int foo(float x, row_major float y : VPOS = 7.0f) { return x + y; }
    public record FunctionDeclaration : Statement {
        public Type                    returnType     { get; set; }
        public Identifier          id             { get; set; }
        public ArgumentList<Parameter> parameters     { get; set; }
        public Semantic?               returnSemantic { get; set; }
        public Block                   body           { get; set; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new HlslSyntax[]
                { returnType, id, parameters, returnSemantic, body }
            .FilterNotNull().ToList();

        /// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-function-parameters">HLSL parameter syntax</a>
        public record Parameter : HlslSyntax {
            public HlslToken?     modifier    { get; set; }
            public Type           type        { get; set; }
            public Identifier id          { get; set; }
            public Semantic?      semantic    { get; set; }
            public Expression?    initializer { get; set; }

            public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new IHlslSyntaxOrToken[]
                    { modifier, type, id, semantic, initializer }
                .FilterNotNull().ToList();
        }
    }
}

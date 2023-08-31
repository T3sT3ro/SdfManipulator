#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements.Declarations {
    // int foo(float x, row_major float y : VPOS = 7.0f) { return x + y; }
    public partial record FunctionDeclaration : Statement {
        private readonly Type                    _returnType;
        private readonly Identifier              _id;
        private readonly ArgumentList<Parameter> _paramList;
        private readonly Semantic?               _returnSemantic;
        private readonly Block                   _body;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new Syntax<Hlsl>?[]
                { returnType, id, paramList, returnSemantic, body }
            .FilterNotNull().ToList();

        /// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-function-parameters">HLSL parameter syntax</a>
        public partial record Parameter : Syntax<Hlsl> {
            private readonly Token<Hlsl>? _modifier;
            private readonly Type         _type;
            private readonly Identifier   _id;
            private readonly Semantic?    _semantic;
            private readonly Expression?  _initializer;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
                    { modifier, type, id, semantic, initializer }
                .FilterNotNull().ToList();
        }
    }
}

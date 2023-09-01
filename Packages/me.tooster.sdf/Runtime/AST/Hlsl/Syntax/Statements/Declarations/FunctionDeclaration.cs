#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements.Declarations {
    // int foo(float x, row/*_*/major float y : VPOS = 7.0f) { return x + y; }
    public partial record FunctionDeclaration : Statement {
        private readonly Type                                          /*_*/returnType;
        private readonly Identifier                                    /*_*/id;
        private readonly ArgumentList<FunctionDeclaration.Parameter> /*_*/paramList;
        private readonly Semantic?                                                      /*_*/returnSemantic;
        private readonly Block                                                          /*_*/body;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new Syntax<Hlsl>?[]
                { returnType, id, paramList, returnSemantic, body }
            .FilterNotNull().ToList();

        /// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-function-parameters">HLSL parameter syntax</a>
        public partial record Parameter : Syntax<Hlsl> {
            private readonly Token<Hlsl>?                /*_*/modifier;
            private readonly Type       /*_*/type;
            private readonly Identifier /*_*/id;
            private readonly Semantic?  /*_*/semantic;
            private readonly Expression?                 /*_*/initializer;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
                    { modifier, type, id, semantic, initializer }
                .FilterNotNull().ToList();
        }
    }
}

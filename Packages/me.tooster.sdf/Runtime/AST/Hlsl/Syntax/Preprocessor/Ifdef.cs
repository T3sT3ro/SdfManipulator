using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    public partial record Ifdef : PreprocessorSyntax {
        private readonly IfdefKeyword                /*_*/ifdefKeyword;
        private readonly Identifier /*_*/id;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, ifdefKeyword, id };
    }
}

using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    public partial record Endif : PreprocessorSyntax {
        private readonly EndIfKeyword /*_*/endifKeyword;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, endifKeyword };
    }
}

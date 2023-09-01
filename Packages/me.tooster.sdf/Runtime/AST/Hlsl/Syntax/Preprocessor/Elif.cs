using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    public partial record Elif : PreprocessorSyntax {
        private readonly ElifKeyword /*_*/elifKeyword;
        private readonly TokenString /*_*/condition;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, elifKeyword, condition };
    }
}

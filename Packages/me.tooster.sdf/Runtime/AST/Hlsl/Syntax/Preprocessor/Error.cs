using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    public partial record Error : PreprocessorSyntax {
        private readonly ErrorKeyword /*_*/errorKeyword;
        private readonly TokenString  /*_*/tokenstring;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, errorKeyword, tokenstring };
    }
}

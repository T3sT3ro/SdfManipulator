using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    public partial record If : PreprocessorSyntax {
        private readonly IfKeyword   /*_*/ifKeyword;
        private readonly TokenString /*_*/condition;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, ifKeyword, condition };
    }
}

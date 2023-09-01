using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    public partial record Ifndef : PreprocessorSyntax {
        private readonly IfndefKeyword               /*_*/ifndefKeyword;
        private readonly Identifier /*_*/id;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, ifndefKeyword, id };
    }
}

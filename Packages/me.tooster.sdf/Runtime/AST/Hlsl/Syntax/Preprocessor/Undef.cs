using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    public partial record Undef : PreprocessorSyntax {
        private readonly UndefKeyword                /*_*/undefKeyword;
        private readonly Identifier /*_*/id;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, undefKeyword, id };
    }
}

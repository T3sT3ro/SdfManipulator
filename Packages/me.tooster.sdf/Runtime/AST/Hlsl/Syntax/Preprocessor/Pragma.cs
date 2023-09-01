#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    public partial record Pragma : PreprocessorSyntax {
        private readonly PragmaKeyword /*_*/pragmaKeyword;
        private readonly TokenString?  /*_*/tokenString;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
            { hashToken, pragmaKeyword, tokenString }.FilterNotNull().ToList();
    }
}

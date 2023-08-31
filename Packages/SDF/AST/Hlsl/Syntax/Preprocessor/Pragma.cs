#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public partial record Pragma : PreprocessorSyntax {
        private readonly PragmaKeyword _pragmaKeyword;
        private readonly TokenString?  _tokenString;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
            { hashToken, pragmaKeyword, tokenString }.FilterNotNull().ToList();
    }
}

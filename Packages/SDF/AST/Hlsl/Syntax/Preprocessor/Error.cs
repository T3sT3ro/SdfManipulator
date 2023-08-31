using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public partial record Error : PreprocessorSyntax {
        private readonly ErrorKeyword _errorKeyword;
        private readonly TokenString  _tokenstring;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, errorKeyword, tokenstring };
    }
}

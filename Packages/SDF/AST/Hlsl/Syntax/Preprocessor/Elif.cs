using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public partial record Elif : PreprocessorSyntax {
        private readonly ElifKeyword _elifKeyword;
        private readonly TokenString _condition;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, elifKeyword, condition };
    }
}

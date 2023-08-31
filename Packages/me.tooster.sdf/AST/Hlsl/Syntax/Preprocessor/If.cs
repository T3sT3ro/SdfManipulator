using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public partial record If : PreprocessorSyntax {
        private readonly IfKeyword   _ifKeyword;
        private readonly TokenString _condition;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, ifKeyword, condition };
    }
}

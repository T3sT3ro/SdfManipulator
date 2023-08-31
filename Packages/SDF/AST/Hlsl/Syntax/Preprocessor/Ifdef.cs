using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public partial record Ifdef : PreprocessorSyntax {
        private readonly IfdefKeyword _ifdefKeyword;
        private readonly Identifier   _id;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, ifdefKeyword, id };
    }
}

using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public partial record Undef : PreprocessorSyntax {
        private readonly UndefKeyword _undefKeyword;
        private readonly Identifier   _id;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, undefKeyword, id };
    }
}

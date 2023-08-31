using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public partial record Ifndef : PreprocessorSyntax {
        private readonly IfndefKeyword _ifndefKeyword;
        private readonly Identifier    _id;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { hashToken, ifndefKeyword, id };
    }
}

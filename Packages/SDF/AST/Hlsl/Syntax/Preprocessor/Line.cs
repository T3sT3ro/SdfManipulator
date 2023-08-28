#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public record Line : PreprocessorSyntax {
        public LineKeyword          lineKeyword { get; init; } = new();
        public IntLiteral           lineNumber  { get; init; }
        public QuotedStringLiteral? file        { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
            { hashToken, lineKeyword, lineNumber, file }.FilterNotNull().ToList();
    }
}

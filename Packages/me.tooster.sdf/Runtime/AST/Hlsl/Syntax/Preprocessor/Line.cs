#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [SyntaxNode] public partial record Line : PreprocessorSyntax {
        public LineKeyword          lineKeyword { get; init; } = new();
        public IntLiteral           lineNumber  { get; init; }
        public QuotedStringLiteral? file        { get; init; }
    }
}

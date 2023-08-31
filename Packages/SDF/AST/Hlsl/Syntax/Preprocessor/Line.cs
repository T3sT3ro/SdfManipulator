#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public partial record Line : PreprocessorSyntax {
        private readonly LineKeyword          _lineKeyword;
        private readonly IntLiteral           _lineNumber;
        private readonly QuotedStringLiteral? _file;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
            { hashToken, lineKeyword, lineNumber, file }.FilterNotNull().ToList();
    }
}

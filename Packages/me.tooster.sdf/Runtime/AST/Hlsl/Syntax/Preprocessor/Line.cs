#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    public partial record Line : PreprocessorSyntax {
        private readonly LineKeyword          /*_*/lineKeyword;
        private readonly IntLiteral           /*_*/lineNumber;
        private readonly QuotedStringLiteral? /*_*/file;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
            { hashToken, lineKeyword, lineNumber, file }.FilterNotNull().ToList();
    }
}

#nullable enable
using System;
using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl.Syntax.Statements {
    public record Return : Statement {
        public ReturnKeyword returnKeyword { get; set; }
        public Expression?   expression    { get; set; }

        public override IReadOnlyList<HlslSyntax> ChildNodes =>
            expression == null ? Array.Empty<Expression>() : new[] { expression };
        
        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens => expression == null
            ? new HlslSyntaxOrToken[] { returnKeyword }
            : new HlslSyntaxOrToken[] { returnKeyword, expression };
    }
}
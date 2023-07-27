using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    public record Comma : Expression {
        public Expression left  { get; set; }
        public CommaToken comma { get; set; }
        public Expression right { get; set; }


        public override IReadOnlyList<HlslSyntax>        ChildNodes          => new[] { left, right };
        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens => new HlslSyntaxOrToken[] { left, comma, right };
    }
}

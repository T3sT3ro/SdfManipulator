using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    public record AssignmentExpresion : Expression {
        public Expression left            { get; internal set; }
        public HlslToken  assignmentToken { get; internal set; }
        public Expression right           { get; internal set; }

        public override IReadOnlyList<HlslSyntax>        ChildNodes          => new[] { left, right };
        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens => new HlslSyntaxOrToken[] { left, assignmentToken, right };
    }
}

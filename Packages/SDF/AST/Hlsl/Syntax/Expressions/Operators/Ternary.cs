using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    public record Ternary : Expression {
        public Expression    condition     { get; set; }
        public QuestionToken questionToken { get; set; }
        public Expression    whenTrue      { get; set; }
        public ColonToken    colonToken    { get; set; }
        public Expression    whenFalse     { get; set; }

        public override IReadOnlyList<HlslSyntax> ChildNodes =>
            new HlslSyntax[] { condition, whenTrue, whenFalse };

        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens => new HlslSyntaxOrToken[]
            { condition, questionToken, whenTrue, colonToken, whenFalse };
    }
}

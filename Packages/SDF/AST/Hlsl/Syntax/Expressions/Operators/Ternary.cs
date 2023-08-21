using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    public record Ternary : Expression {
        public Expression    condition     { get; set; }
        public QuestionToken questionToken { get; set; } = new();
        public Expression    whenTrue      { get; set; }
        public ColonToken    colonToken    { get; set; } = new();
        public Expression    whenFalse     { get; set; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new IHlslSyntaxOrToken[]
            { condition, questionToken, whenTrue, colonToken, whenFalse };
    }
}

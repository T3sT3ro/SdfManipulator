using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    public record Cast : Expression {
        public OpenParenToken  openParenToken  { get; set; }
        public Type       type            { get; set; }
        public CloseParenToken closeParenToken { get; set; }
        public Expression      expression      { get; set; }
        
        public override IReadOnlyList<HlslSyntax>        ChildNodes          { get; }
        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens { get; }
    }
}
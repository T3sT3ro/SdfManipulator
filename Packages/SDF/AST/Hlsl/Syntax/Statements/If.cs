#nullable enable
using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl.Syntax.Statements {
    public record If : HlslSyntax {
        public IfKeyword       ifKeyword  { get; set; }
        public OpenParenToken  openParen  { get; set; }
        public Expression      test       { get; set; }
        public CloseParenToken closeParen { get; set; }
        public Block?          body       { get; set; }
        public Block?          @else      { get; set; }

        public override IReadOnlyList<HlslSyntax> ChildNodes => new HlslSyntax[]
            { test, body, @else };

        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens => new HlslSyntaxOrToken[]
            { ifKeyword, openParen, test, closeParen, body, @else };
    }
}

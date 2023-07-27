using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    public record Member : Expression {
        public          Expression                       expression          { get; set; }
        public DotToken                         dotToken            { get; set; }
        public          IdentifierName                   member              { get; set; }
        
        public override IReadOnlyList<HlslSyntax>        ChildNodes          => new HlslSyntax[] { expression, member };
        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens => new HlslSyntaxOrToken[] { expression, dotToken, member };
    }
}
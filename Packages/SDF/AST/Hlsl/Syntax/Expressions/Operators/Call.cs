#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    //. Known as InvocationExpression 
    public record Call : Expression {
        public IdentifierName id        { get; set; }
        public ArgumentList   arguments { get; set; }

        public override IReadOnlyList<HlslSyntax> ChildNodes =>
            (IReadOnlyList<HlslSyntax>)new[] { id }.Concat<HlslSyntax>(arguments.ChildNodes);

        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens =>
            (IReadOnlyList<HlslSyntaxOrToken>)new[] { id }.Concat(arguments.ChildNodesAndTokens);
    }
}

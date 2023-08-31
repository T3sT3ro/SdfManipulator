using System.Collections.Generic;
using AST.Syntax;
using SDF.SourceGen;

namespace AST.Hlsl.Syntax.Expressions {
    [AstSyntax]
    public partial record LiteralExpression<T> : Expression where T : Literal {
        private readonly T _literal;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new[]
            { literal };
    }
}

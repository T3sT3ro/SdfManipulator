#nullable enable
using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public partial record If {
        public record ElseClause : Syntax<Hlsl> {
            public ElseKeyword elseKeyword { get; init; } = new();
            public Statement   statement   { get; init; }

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
                { elseKeyword, statement };
        }
    }
}

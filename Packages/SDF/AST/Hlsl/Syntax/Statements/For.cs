#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    // for (start conditions; conditions; increments) { body }
    // start conditions:
    //    declarations: int i, j = 0;
    //    initializers: i = 0, j = 0;
    public partial record For(Statement body) : Statement {
        public ForKeyword                       forKeyword      { get; init; } = new();
        public OpenParenToken                   openParen       { get; init; } = new();
        public Initializer?                     initializer     { get; init; }
        public SemiToken                        firstSemiToken  { get; init; } = new();
        public Expression?                      condition       { get; init; }
        public SemiToken                        secondSemiToken { get; init; } = new();
        public SeparatedList<Hlsl, Expression>? increments      { get; init; }
        public CloseParenToken                  closeParen      { get; init; } = new();
        public Statement                        body            { get; init; } = body;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
            {
                forKeyword, openParen, initializer, firstSemiToken, condition, secondSemiToken, increments, closeParen,
                body,
            }.FilterNotNull()
            .ToList();

        public abstract record Initializer : Syntax<Hlsl>;
    }
}

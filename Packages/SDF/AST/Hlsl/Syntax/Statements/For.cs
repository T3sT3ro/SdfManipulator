#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Hlsl.Syntax.Expressions;
using AST.Hlsl.Syntax.Expressions.Operators;
using AST.Hlsl.Syntax.Statements.Declarations;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    // for (start conditions; conditions; increments) { body }
    // start conditions:
    //    declarations: int i, j = 0;
    //    initializers: i = 0, j = 0;
    public record For(Statement body) : Statement {
        public ForKeyword                 forKeyword      { get; init; } = new();
        public OpenParenToken             openParen       { get; init; } = new();
        public ForInitializer?            initializer     { get; init; }
        public SemiToken                  firstSemiToken  { get; init; } = new();
        public Expression?                condition       { get; init; }
        public SemiToken                  secondSemiToken { get; init; } = new();
        public SeparatedList<Expression>? increments      { get; init; }
        public CloseParenToken            closeParen      { get; init; } = new();
        public Statement                  body            { get; init; } = body;

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new IHlslSyntaxOrToken[]
                { forKeyword, openParen }
            .AppendNotNull(initializer)
            .AppendNotNull(firstSemiToken)
            .AppendNotNull(condition)
            .Append(secondSemiToken)
            .AppendNotNull(increments)
            .Append(closeParen)
            .Append(body)
            .ToArray();
    }

    public abstract record ForInitializer : HlslSyntax;

    public record ForVariableDeclarator : ForInitializer {
        public VariableDeclarator declarator { get; internal init; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new HlslSyntax[] { declarator };
    }

    public record ForVariableInitializer : ForInitializer {
        public SeparatedList<AssignmentExpresion> initializers { get; init; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => initializers;
    }
}

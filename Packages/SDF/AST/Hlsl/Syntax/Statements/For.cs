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
        public ForKeyword                 forKeyword      { get; set; } = new();
        public OpenParenToken             openParen       { get; set; } = new();
        public ForInitializer?            initializer     { get; set; }
        public SemiToken                  firstSemiToken  { get; set; } = new();
        public Expression?                condition       { get; set; }
        public SemiToken                  secondSemiToken { get; set; } = new();
        public SeparatedList<Expression>? increments      { get; set; }
        public CloseParenToken            closeParen      { get; set; } = new();
        public Statement                  body            { get; set; } = body;

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
        public VariableDeclarator declarator { get; internal set; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new HlslSyntax[] { declarator };
    }

    public record ForVariableInitializer : ForInitializer {
        public SeparatedList<AssignmentExpresion> initializers { get; set; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => initializers;
    }
}

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using AST.Hlsl.Syntax.Expressions;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public record For : Statement {
        public ForKeyword     forKeyword { get; set; }
        public OpenParenToken openParen  { get; set; }

        public HlslSeparatedSyntaxList declarations    { get; set; }
        public HlslSeparatedSyntaxList initializers    { get; set; }
        public SemiToken               firstSemiToken  { get; set; }
        public HlslSeparatedSyntaxList conditions      { get; set; }
        public SemiToken               secondSemiToken { get; set; }
        public HlslSeparatedSyntaxList increments      { get; set; }

        public CloseParenToken closeParen { get; set; }
        public Block           body       { get; set; }

        public override IReadOnlyList<HlslSyntax> ChildNodes =>
            declarations
                .Concat(initializers)
                .Concat(conditions)
                .Concat(increments)
                .Append(body)
                .ToList();


        public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens =>
            new HlslSyntaxOrToken[] { forKeyword, openParen }
                .Concat(declarations)
                .Concat(initializers)
                .Append(firstSemiToken)
                .Concat(conditions)
                .Append(secondSemiToken)
                .Concat(increments)
                .Append(closeParen)
                .Append(body)
                .ToArray();
    }
}

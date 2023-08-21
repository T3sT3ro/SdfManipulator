using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    // someFunction(arguments) 
    public record Call : Expression {
        public Identifier               id        { get; init; }
        public ArgumentList<HlslSyntax> arguments { get; init; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new HlslSyntax[] { id, arguments };
    }
}

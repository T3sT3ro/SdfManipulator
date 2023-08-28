using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    // someFunction(arguments) 
    public record Call : Expression {
        public Identifier                 id      { get; init; }
        public ArgumentList<Syntax<Hlsl>> argList { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new Syntax<Hlsl>[]
            { id, argList };
    }
}

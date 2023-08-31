using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Expressions.Operators {
    // someFunction(arguments) 
    public partial record Call : Expression {
        private readonly Identifier                 _id;
        private readonly ArgumentList<Syntax<Hlsl>> _argList;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new Syntax<Hlsl>[]
            { id, argList };
    }
}

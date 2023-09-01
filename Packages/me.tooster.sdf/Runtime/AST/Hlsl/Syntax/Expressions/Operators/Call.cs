using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators {
    // someFunction(arguments) 
    public partial record Call : Expression {
        private readonly Identifier  /*_*/id;
        private readonly ArgumentList<Syntax<Hlsl>> /*_*/argList;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new Syntax<Hlsl>[]
            { id, argList };
    }
}

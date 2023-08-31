#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public partial record Return : Statement {
        private readonly ReturnKeyword _returnKeyword;
        private readonly Expression?   _expression;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[] 
                { returnKeyword, expression }.FilterNotNull().ToList();
    }
}

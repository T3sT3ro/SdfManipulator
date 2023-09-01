#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record Return : Statement {
        private readonly ReturnKeyword /*_*/returnKeyword;
        private readonly Expression?   /*_*/expression;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[] 
                { returnKeyword, expression }.FilterNotNull().ToList();
    }
}

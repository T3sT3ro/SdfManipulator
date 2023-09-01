using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    public partial record Typedef : Statement {
        private readonly TypedefKeyword              /*_*/typedefKeyword;
        private readonly Type       /*_*/type;
        private readonly Identifier /*_*/id;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { typedefKeyword, type, id };
    }
}

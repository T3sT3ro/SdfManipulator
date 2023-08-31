using System.Collections.Generic;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Statements {
    public partial record Typedef : Statement {
        private readonly TypedefKeyword _typedefKeyword;
        private readonly Type           _type;
        private readonly Identifier     _id;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>[]
            { typedefKeyword, type, id };
    }
}

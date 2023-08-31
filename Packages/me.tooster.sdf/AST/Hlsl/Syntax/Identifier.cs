using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions;
using AST.Syntax;

namespace AST.Hlsl.Syntax {
    public partial record Identifier : Expression {
        private readonly IdentifierToken _id;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new[]
            { id };

        public static implicit operator Identifier(string name) =>
            new() { id = new IdentifierToken { ValidatedText = name } };
    }
}

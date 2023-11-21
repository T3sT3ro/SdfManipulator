using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    [SyntaxNode] public partial record Identifier : Expression {
        public IdentifierToken id { get; init; }

        public static implicit operator Identifier(string name) =>
            new() { id = new IdentifierToken { ValidatedText = name } };
    }
}

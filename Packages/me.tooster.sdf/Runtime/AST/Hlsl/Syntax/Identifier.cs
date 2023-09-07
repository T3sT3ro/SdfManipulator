using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    [Syntax] public partial record Identifier : Expression {
        private readonly IdentifierToken _id;

        public static implicit operator Identifier(string name) =>
            new() { id = new IdentifierToken { ValidatedText = name } };
    }
}

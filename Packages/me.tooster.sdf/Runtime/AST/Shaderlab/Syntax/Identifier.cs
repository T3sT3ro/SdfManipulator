using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.shaderlab>;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    [SyntaxNode] public partial record Identifier : Expression {
        public IdentifierToken id { get; init; }

        public static implicit operator Identifier(string name) =>
            new() { id = new IdentifierToken { ValidatedText = name } };
        
        public static implicit operator Identifier(IdentifierToken token) =>
            new() { id = token };
    }
}

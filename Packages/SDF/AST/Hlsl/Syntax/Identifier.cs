using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl.Syntax {
    public record Identifier : Expression {
        public IdentifierToken id { get; init; }

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => new IHlslSyntaxOrToken[] { id };
        
        public static implicit operator Identifier(string name) => new() { id = new() { ValidatedText = name } }; 
    }
}

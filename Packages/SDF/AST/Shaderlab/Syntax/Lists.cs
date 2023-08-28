using System.Collections.Generic;
using AST.Syntax;

namespace AST.Shaderlab.Syntax {
    // ( SYNTAX TOKEN SYNTAX TOKEN )
    public record ArgumentList<TSyntax> : Syntax<Shaderlab> where TSyntax : Syntax<Shaderlab> {
        public OpenParenToken                    openParenToken  { get; init; } = new();
        public SeparatedList<Shaderlab, TSyntax> arguments       { get; init; }
        public CloseParenToken                   closeParenToken { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
            { openParenToken, arguments, closeParenToken };

        public static implicit operator ArgumentList<TSyntax>(TSyntax[] arguments) => new ArgumentList<TSyntax>
            { arguments = SeparatedList<Shaderlab, TSyntax>.With<CommaToken>(arguments) };
    }
}

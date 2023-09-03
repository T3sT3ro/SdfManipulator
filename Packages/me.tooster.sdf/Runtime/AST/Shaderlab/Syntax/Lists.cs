using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    // ( SYNTAX TOKEN SYNTAX TOKEN )
    public record ArgumentList<TSyntax> : Syntax<Shaderlab> where TSyntax : Syntax<Shaderlab> {
        public OpenParenToken                    _openParenToken  { get; init; } = new();
        public SeparatedList<Shaderlab, TSyntax> _arguments       { get; init; }
        public CloseParenToken                   _closeParenToken { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Shaderlab>> ChildNodesAndTokens => new SyntaxOrToken<Shaderlab>[]
            { openParenToken, arguments, closeParenToken };

        public static implicit operator ArgumentList<TSyntax>(TSyntax[] arguments) => new ArgumentList<TSyntax>
            { arguments = SeparatedList<Shaderlab, TSyntax>.With<CommaToken>(arguments) };
    }
}

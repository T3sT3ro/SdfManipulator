using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    // ( SYNTAX TOKEN SYNTAX TOKEN )
    [AstSyntax] public partial record ArgumentList<TSyntax> : Syntax<Shaderlab> where TSyntax : Syntax<Shaderlab> {
        public OpenParenToken                    openParenToken { get; init; } = new();
        public SeparatedList<Shaderlab, TSyntax> arguments { get; init; } = new();
        public CloseParenToken                   closeParenToken { get; init; } = new();

        public static implicit operator ArgumentList<TSyntax>(TSyntax[] arguments) => new ArgumentList<TSyntax>
            { arguments = SeparatedList<Shaderlab, TSyntax>.With<CommaToken>(arguments) };
    }
}

using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    // ( SYNTAX TOKEN SYNTAX TOKEN )
    [Syntax] public partial record  ArgumentList<TSyntax> : Syntax<Shaderlab> where TSyntax : Syntax<Shaderlab> {
        [Init] private readonly  OpenParenToken                    _openParenToken  ;
        [Init] private readonly  SeparatedList<Shaderlab, TSyntax> _arguments       ;
        [Init] private readonly  CloseParenToken                   _closeParenToken ;

        public static implicit operator ArgumentList<TSyntax>(TSyntax[] arguments) => new ArgumentList<TSyntax>
            { arguments = SeparatedList<Shaderlab, TSyntax>.With<CommaToken>(arguments) };
    }
}

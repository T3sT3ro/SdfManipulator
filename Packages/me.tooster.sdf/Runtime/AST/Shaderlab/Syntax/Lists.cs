using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    // ( SYNTAX TOKEN SYNTAX TOKEN )
    [SyntaxNode] public partial record ArgumentList<TSyntax> : Syntax<shaderlab> where TSyntax : Syntax<shaderlab> {
        public OpenParenToken                    openParenToken  { get; init; } = new();
        public SeparatedList<shaderlab, TSyntax> arguments       { get; init; } = new();
        public CloseParenToken                   closeParenToken { get; init; } = new();

        public static implicit operator ArgumentList<TSyntax>(TSyntax[] arguments) => new()
            { arguments = SeparatedList<shaderlab, TSyntax>.WithSeparator<CommaToken>(arguments) };
    }
}

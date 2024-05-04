using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    public interface IArgumentList {
        public OpenParenToken  openParenToken  { get; init; }
        public ISeparatedList  arguments       { get; }
        public CloseParenToken closeParenToken { get; init; }
    }



    /// ( SYNTAX TOKEN SYNTAX TOKEN )
    [SyntaxNode] public partial record ArgumentList<TSyntax> : Syntax<shaderlab>, IArgumentList where TSyntax : Syntax<shaderlab> {
        public OpenParenToken                    openParenToken  { get; init; } = new();
        public SeparatedList<shaderlab, TSyntax> arguments       { get; init; } = new();
        ISeparatedList IArgumentList.            arguments       => arguments;
        public CloseParenToken                   closeParenToken { get; init; } = new();

        public static implicit operator ArgumentList<TSyntax>(TSyntax[] arguments)
            => new() { arguments = SeparatedList<shaderlab, TSyntax>.WithSeparator<CommaToken>(arguments) };

        public static implicit operator ArgumentList<TSyntax>(TSyntax singletonElement)
            => new() { arguments = SeparatedList<shaderlab, TSyntax>.WithSeparator<CommaToken>(singletonElement) };

        public static implicit operator ArgumentList<TSyntax>(SeparatedList<shaderlab, TSyntax> separatedList)
            => new() { arguments = separatedList };

        public ArgumentList(params TSyntax[] elements) => arguments = elements.CommaSeparated();
    }
}

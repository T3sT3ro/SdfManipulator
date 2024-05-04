using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
namespace me.tooster.sdf.AST.Hlsl.Syntax {
    public interface IArgumentList {
        public OpenParenToken  openParenToken  { get; init; }
        public ISeparatedList  arguments       { get; }
        public CloseParenToken closeParenToken { get; init; }
    }



    /// ( T , ... , T ) 
    [SyntaxNode] public partial record ArgumentList<T> : Syntax<hlsl>, IArgumentList where T : Syntax<hlsl> {
        public OpenParenToken         openParenToken  { get; init; } = new();
        public SeparatedList<hlsl, T> arguments       { get; init; } = new();
        ISeparatedList IArgumentList. arguments       => arguments;
        public CloseParenToken        closeParenToken { get; init; } = new();

        public static implicit operator ArgumentList<T>(T[] args) => new() { arguments = args.CommaSeparated() };
        public static implicit operator ArgumentList<T>(T singleton) => new() { arguments = singleton.CommaSeparated() };
        public static implicit operator ArgumentList<T>(SeparatedList<hlsl, T> separatedList) => new() { arguments = separatedList };
    }



    /// <inheritdoc cref="BracedList{T}"/>
    public interface IBracedList {
        public OpenBraceToken  openBraceToken  { get; init; }
        public ISeparatedList  arguments       { get; }
        public CloseBraceToken closeBraceToken { get; init; }
    }



    /// { T , ... , T }
    [SyntaxNode] public partial record BracedList<T> : Syntax<hlsl>, IBracedList where T : Syntax<hlsl> {
        public OpenBraceToken         openBraceToken { get; init; } = new();
        public SeparatedList<hlsl, T> arguments      { get; init; } = new();
        ISeparatedList IBracedList.   arguments      => arguments;

        public CloseBraceToken closeBraceToken { get; init; } = new();

        public static implicit operator BracedList<T>(SyntaxOrToken<hlsl>[] args) => new() { arguments = new SeparatedList<hlsl, T>(args) };
        public static implicit operator BracedList<T>(T[] args) => new() { arguments = args.CommaSeparated() };
        public static implicit operator BracedList<T>(T singleton) => new() { arguments = singleton.CommaSeparated() };
        public static implicit operator BracedList<T>(SeparatedList<hlsl, T> separatedList) => new() { arguments = separatedList };
    }
}

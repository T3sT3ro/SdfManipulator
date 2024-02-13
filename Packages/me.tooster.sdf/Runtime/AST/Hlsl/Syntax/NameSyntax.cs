using me.tooster.sdf.AST.Syntax.CommonSyntax;
namespace me.tooster.sdf.AST.Hlsl.Syntax {
    [SyntaxNode] public abstract partial record NameSyntax : Expression<hlsl> {

        public static implicit operator NameSyntax(string name) => Extensions.NameFrom(name);
    }

    [SyntaxNode] public partial record QualifiedNameSyntax : NameSyntax {
        public NameSyntax       Left            { get; init; }
        public ColonColonToken  colonColonToken { get; init; } = new();
        public SimpleNameSyntax Right           { get; init; }
    }
    [SyntaxNode] public abstract partial record SimpleNameSyntax : NameSyntax;
}

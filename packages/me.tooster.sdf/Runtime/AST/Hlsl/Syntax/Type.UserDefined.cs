#nullable enable
namespace me.tooster.sdf.AST.Hlsl.Syntax {
    public abstract partial record Type {
        /// struct names or types introduced by typedef
        [SyntaxNode] public partial record UserDefined : Type {
            public Identifier id { get; init; }

            public static implicit operator UserDefined(string name) => new() { id = name };
        }
    }
}

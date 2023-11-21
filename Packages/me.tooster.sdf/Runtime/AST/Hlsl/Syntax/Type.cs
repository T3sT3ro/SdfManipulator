#nullable enable
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    [SyntaxNode] public abstract partial record Type : Syntax<hlsl> {
        public static implicit operator Type(PredefinedTypeToken token) => (Predefined)token;
        public static implicit operator Type(string name)               => (UserDefined)name;
    }
}

#nullable enable
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    [AstSyntax] public abstract partial record Type : Syntax<Hlsl> {
        public static implicit operator Type(PredefinedTypeToken token) => (Predefined)token;
        public static implicit operator Type(string name)               => (UserDefined)name;
    }
}

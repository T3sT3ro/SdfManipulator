#nullable enable
using AST.Syntax;

namespace AST.Hlsl.Syntax {
    public abstract partial record Type : Syntax<Hlsl> {
        public static implicit operator Type(PredefinedTypeToken token) => (Predefined)token;

        public static implicit operator Type(string name) => (UserDefined)name;
    }
}

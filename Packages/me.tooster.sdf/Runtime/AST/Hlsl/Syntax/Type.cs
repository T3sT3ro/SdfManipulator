#nullable enable
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    public abstract partial record Type : Syntax<Hlsl> {
        protected Type() : base() { }
        public static implicit operator Type(PredefinedTypeToken token) => (Type.Predefined)token;

        public static implicit operator Type(string name) => (Type.UserDefined)name;
    }
}

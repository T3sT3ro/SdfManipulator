#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    public abstract partial record Type {
        /// struct names or types introduced by typedef
        [Syntax] public partial record UserDefined : Type {
            private readonly Identifier _id;

            public static implicit operator UserDefined(string name) => new() { id = name };
        }
    }
}

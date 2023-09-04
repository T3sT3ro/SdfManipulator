#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    public abstract partial record Type {
        // anonymous and empty: struct {}
        // regular: struct x { float pos; x other; }
        // Struct shape is a valid type and can be used in function parameter or variable declarations.
        [Syntax] public partial record Struct : Type {
            [Init] private readonly StructKeyword            _structKeyword;
            private readonly        Identifier?              _name;
            [Init] private readonly OpenBraceToken           _openBrace;
            [Init] private readonly SyntaxList<Hlsl, Member> _members;
            [Init] private readonly CloseBraceToken          _closeBrace;

            [Syntax] public partial record Member : Syntax<Hlsl> {
                private readonly        Token<Hlsl>?   _interpolation;
                private readonly        Type           _type;
                private readonly        Identifier     _id;
                private readonly        Semantic?      _semantic;
                [Init] private readonly SemicolonToken _semicolon;
            }
        }
    }
}

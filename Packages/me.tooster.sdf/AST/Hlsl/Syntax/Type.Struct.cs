#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax {
    public abstract partial record Type {
        // anonymous and empty: struct {}
        // regular: struct x { float pos; x other; }
        // Struct shape is a valid type and can be used in function parameter or variable declarations.  
        public partial record Struct : Type {
            private readonly StructKeyword            _structKeyword;
            private readonly Identifier?              _name;
            private readonly OpenBraceToken           _openBrace;
            private readonly SyntaxList<Hlsl, Member> _members;
            private readonly CloseBraceToken          _closeBrace;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
                    { structKeyword, name, openBrace, members, closeBrace }
                .FilterNotNull().ToList();

            public partial record Member : Syntax<Hlsl> {
                private readonly Token<Hlsl>? _interpolation;
                private readonly Type         _type;
                private readonly Identifier   _id;
                private readonly Semantic?    _semantic;
                private readonly SemiToken    _semicolon;

                public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
                        { interpolation, type, id, semantic, semicolon }
                    .FilterNotNull().ToList();
            }
        }
    }
}

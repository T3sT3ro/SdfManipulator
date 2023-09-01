#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
    public abstract partial record Type {
        // anonymous and empty: struct {}
        // regular: struct x { float pos; x other; }
        // Struct shape is a valid type and can be used in function parameter or variable declarations.  
        public partial record Struct : Type {
            private readonly StructKeyword                   /*_*/structKeyword;
            private readonly Identifier?    /*_*/name;
            private readonly OpenBraceToken                  /*_*/openBrace;
            private readonly SyntaxList<Hlsl, Struct.Member> /*_*/members;
            private readonly CloseBraceToken                 /*_*/closeBrace;

            public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
                    { structKeyword, name, openBrace, members, closeBrace }
                .FilterNotNull().ToList();

            public partial record Member : Syntax<Hlsl> {
                private readonly Token<Hlsl>?                /*_*/interpolation;
                private readonly Type       /*_*/type;
                private readonly Identifier /*_*/id;
                private readonly Semantic?  /*_*/semantic;
                private readonly SemiToken                   /*_*/semicolon;

                public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
                        { interpolation, type, id, semantic, semicolon }
                    .FilterNotNull().ToList();
            }
        }
    }
}

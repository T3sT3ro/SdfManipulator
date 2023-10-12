using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Foo.NamespaceSyntax {
    [AstSyntax] public partial record OuterSyntax : Syntax<Foo> {
        public InnerSyntax innerSyntax { get; init; }
        
        [AstSyntax] public partial record InnerSyntax : Syntax<Foo> {
            public OuterSyntax?
                circular { get; init; }
        }
    }
}

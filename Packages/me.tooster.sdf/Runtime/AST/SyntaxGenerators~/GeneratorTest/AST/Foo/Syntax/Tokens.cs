using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Foo {
    [AstToken] public record OpenParenToken                        : Token<Foo> { public override string Text => "("; }
    [AstToken] public record CloseParenToken                       : Token<Foo> { public override string Text => ")"; }

    [AstToken] public record IntToken : Token<Foo> {
        public override string Text => "0";
    }
}

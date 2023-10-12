using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Foo {
    public interface Foo { }
    
    [AstSyntax] public abstract partial record NarrowedSyntax : Syntax<Foo>;
}

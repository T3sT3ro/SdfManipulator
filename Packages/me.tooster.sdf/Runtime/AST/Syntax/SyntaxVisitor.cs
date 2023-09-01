#nullable enable
namespace me.tooster.sdf.AST.Syntax {
    public abstract class SyntaxVisitor<Lang, TResult> {
        protected virtual TResult? Visit(Syntax<Lang> node) { return default; }
    }

    public abstract class SyntaxVisitor<Lang> {
        protected virtual void Visit(Syntax<Lang> node) { }
    }
}

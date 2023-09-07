using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Trivias {
    public abstract partial record Comment : SimpleTrivia<Hlsl> {
        public record Line : Comment;

        public record Block : Comment;
    }
}

using AST.Syntax;

namespace AST.Hlsl.Syntax.Trivias {
    public abstract record Comment : Trivia<Hlsl> {
        public record Line : Comment;

        public record Block : Comment;
    }
}

using AST.Syntax;

namespace AST.Hlsl.Syntax.Trivias {
    public abstract partial record Comment : Trivia<Hlsl> {
        public partial record Line : Comment;

        public partial record Block : Comment;
    }
}

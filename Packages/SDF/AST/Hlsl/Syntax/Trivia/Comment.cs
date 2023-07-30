namespace AST.Hlsl.Syntax.Trivia {
    public abstract record Comment : HlslTrivia {
        
        public record Line : Comment;
        public record Block : Comment;
    }
}

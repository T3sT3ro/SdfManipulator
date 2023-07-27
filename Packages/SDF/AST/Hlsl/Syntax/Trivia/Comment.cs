namespace AST.Hlsl.Syntax.Trivia {
    public abstract record Comment : HlslTrivia {
        
        public string Text { get; }
        public record Line : Comment;
        public record Block : Comment;
    }
}

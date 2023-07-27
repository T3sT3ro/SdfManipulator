namespace AST.Hlsl.Syntax.Trivia {
    public record Whitespace : HlslTrivia {
        public WhitespaceToken whitespaceToken { get; }
    }
}

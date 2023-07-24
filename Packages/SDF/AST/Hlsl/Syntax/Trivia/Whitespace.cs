namespace AST.Hlsl.Syntax.Trivia {
    public record Whitespace : HlslTrivia {
        public HlslToken.WhitespaceToken Token { get; }
    };
}

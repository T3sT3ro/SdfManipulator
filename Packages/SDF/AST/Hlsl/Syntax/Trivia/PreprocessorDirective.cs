namespace AST.Hlsl.Syntax.Trivia {
    public abstract record PreprocessorDirective : HlslTrivia {
        public HashToken hashToken { get; set; }
    }
}
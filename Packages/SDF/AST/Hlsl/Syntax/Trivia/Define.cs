namespace AST.Hlsl.Syntax.Trivia {
    public record Define : PreprocessorDirective {
        public IdentifierName id     { get; set; }
        public string[]   tokens { get; set; }
    }
}
namespace AST.Hlsl.Syntax.Trivia {
    public record Ifdef : PreprocessorDirective {
        public IdentifierName id { get; set; }
    }
}
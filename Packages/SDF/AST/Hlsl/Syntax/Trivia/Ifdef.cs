namespace AST.Hlsl.Syntax.Trivia {
    public record Ifdef : PreprocessorDirective {
        public Identifier id { get; set; }
    }
}
namespace AST.Hlsl.Syntax.Trivia {
    public record Ifndef : PreprocessorDirective {
        public Identifier id { get; set; }
    }
}
namespace AST.Hlsl.Syntax.Trivia {
    public record Elif : PreprocessorDirective {
        public Identifier id { get; set; }
    }
}
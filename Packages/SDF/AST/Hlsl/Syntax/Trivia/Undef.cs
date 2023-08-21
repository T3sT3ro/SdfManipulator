namespace AST.Hlsl.Syntax.Trivia {
    public record Undef : PreprocessorDirective {
        public Identifier id { get; set; }
    }
}
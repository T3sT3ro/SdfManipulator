namespace AST.Hlsl.Syntax.Trivia {
    public record If : PreprocessorDirective {
        public string[] tokens { get; set; }
    }
}
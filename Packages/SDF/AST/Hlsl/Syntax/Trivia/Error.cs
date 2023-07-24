namespace AST.Hlsl.Syntax.Trivia {
    public record Error : PreprocessorDirective {
        public string[] tokens { get; set; }
    }
}
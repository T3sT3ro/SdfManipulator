namespace AST.Hlsl.Syntax.Trivia {
    public record Include : PreprocessorDirective {
        public string path { get; set; }
    }
}
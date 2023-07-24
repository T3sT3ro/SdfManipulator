namespace AST.Hlsl.Syntax.Trivia {
    public record Pragma : PreprocessorDirective {
            public string[] tokens { get; set; }
        }
}

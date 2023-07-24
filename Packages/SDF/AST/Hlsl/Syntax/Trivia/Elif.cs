namespace AST.Hlsl.Syntax.Trivia {
    public record Elif : PreprocessorDirective {
        public IdentifierName id { get; set; }
    }
}
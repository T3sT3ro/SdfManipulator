namespace AST.Hlsl.Syntax.Trivia {
    public record Ifndef : PreprocessorDirective {
        public IdentifierName id { get; set; }
    }
}
namespace AST.Hlsl.Syntax.Trivia {
    public record Undef : PreprocessorDirective {
        public IdentifierName id { get; set; }
    }
}
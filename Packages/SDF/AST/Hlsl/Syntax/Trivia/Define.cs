namespace AST.Hlsl.Syntax.Trivia {
    public record Define : PreprocessorDirective {
        
        public Identifier id        { get; set; }
        public string[]       tokens    { get; set; }
    }
}
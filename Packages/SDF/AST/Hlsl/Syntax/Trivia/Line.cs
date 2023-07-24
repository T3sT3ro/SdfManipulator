namespace AST.Hlsl.Syntax.Trivia {
    public record Line : PreprocessorDirective {
        public uint   line { get; set; }
        public string file { get; set; }
    }
}
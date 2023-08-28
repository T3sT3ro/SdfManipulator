namespace AST.Syntax {
    public abstract record StructuredTrivia<Lang> : Trivia<Lang> {
        public Syntax<Lang> Structure { get; init; }
    }
}
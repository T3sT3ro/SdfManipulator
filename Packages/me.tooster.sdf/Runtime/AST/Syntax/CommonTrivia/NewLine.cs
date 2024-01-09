namespace me.tooster.sdf.AST.Syntax.CommonTrivia {
    public record NewLine<Lang> : SimpleTrivia<Lang> {
        public override string Text { get; init; } = "\n";
    }
}

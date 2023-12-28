using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Trivias {
    public sealed record NewLine : SimpleTrivia<hlsl> {
        public override string Text { get; init; } = "\n";
    }
}

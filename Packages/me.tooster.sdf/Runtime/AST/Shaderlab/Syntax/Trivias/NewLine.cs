using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Trivias {
    public sealed record NewLine : SimpleTrivia<shaderlab> {
        public override string Text { get; init; } = "\n";
    }
}

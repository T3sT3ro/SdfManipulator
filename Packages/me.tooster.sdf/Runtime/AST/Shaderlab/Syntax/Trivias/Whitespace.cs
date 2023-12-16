using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Trivias {
    public sealed record Whitespace : SimpleTrivia<shaderlab> {
        public Whitespace(string text = " ") { Text = text; }
    }
}

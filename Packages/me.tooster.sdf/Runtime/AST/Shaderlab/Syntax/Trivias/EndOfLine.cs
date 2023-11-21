using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Trivias {
    public record EndOfLine : SimpleTrivia<shaderlab>;

    public static partial class SyntaxFactory {
        public static EndOfLine EndOfLine(int count = 1) => new() { Text = new string('\n', count) };
    }
}

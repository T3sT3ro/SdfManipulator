using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Trivias {
    public record EndOfLine : SimpleTrivia<Shaderlab>;

    public static partial class SyntaxFactory {
        public static EndOfLine EndOfLine(int count = 1) => new EndOfLine { Text = new string('\n', count) };
    }
}

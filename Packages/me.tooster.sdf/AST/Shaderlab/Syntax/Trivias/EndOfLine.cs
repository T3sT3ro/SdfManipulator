namespace AST.Shaderlab.Syntax.Trivias {
    public record EndOfLine : AST.Syntax.Trivia<Shaderlab>;

    public static partial class SyntaxFactory {
        public static EndOfLine EndOfLine(int count = 1) => new EndOfLine { Text = new string('\n', count) };
    }
}
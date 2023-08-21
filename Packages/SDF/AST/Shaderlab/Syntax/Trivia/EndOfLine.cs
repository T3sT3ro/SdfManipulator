using AST.Shaderlab.Syntax.Trivia;

namespace AST.Shaderlab.Syntax.Trivia {
    public record EndOfLine : ShaderlabTrivia;
}

namespace AST.Shaderlab {
    public static partial class SyntaxFactory {
        public static EndOfLine EndOfLine(int count = 1) => new EndOfLine { Text = new string('\n', count) };
    }
}

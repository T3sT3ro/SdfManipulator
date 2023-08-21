using AST.Shaderlab.Syntax.Trivia;

namespace AST.Shaderlab.Syntax.Trivia {
    public record Whitespace : ShaderlabTrivia { }
}

namespace AST.Shaderlab {
    public static partial class SyntaxFactory {
        public static Whitespace Whitespace(string text) => new Whitespace { Text = text };
    }
}

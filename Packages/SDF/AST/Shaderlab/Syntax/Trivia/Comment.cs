using AST.Shaderlab.Syntax.Trivia;

namespace AST.Shaderlab.Syntax.Trivia {
    /// Comment text including the // or /* and */
    public abstract record Comment : ShaderlabTrivia {
        public record Line : Comment;

        public record Block : Comment;
    }
}

namespace AST.Shaderlab {
    public static partial class SyntaxFactory {
        public static Comment Comment(string text) =>
            text.StartsWith("//") ? new Comment.Line { Text = text } : new Comment.Block { Text = text };
    }
}

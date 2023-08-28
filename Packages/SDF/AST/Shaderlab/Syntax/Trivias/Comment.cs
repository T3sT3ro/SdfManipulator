namespace AST.Shaderlab.Syntax.Trivias {
    /// Comment text including the // or /* and */
    public abstract record Comment : AST.Syntax.Trivia<Shaderlab> {
        public record Line : Comment;

        public record Block : Comment;
    }

    public static partial class SyntaxFactory {
        /// Creates appropriate comment type from the comment string. String isn't validated, just checked for initial //
        public static Comment Comment(string text) =>
            text.StartsWith("//") ? new Comment.Line { Text = text } : new Comment.Block { Text = text };
    }
}

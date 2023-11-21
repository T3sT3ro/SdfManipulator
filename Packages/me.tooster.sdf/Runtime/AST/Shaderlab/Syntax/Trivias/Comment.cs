using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Trivias {
    /// Comment text including the // or /* and */
    public abstract record Comment : SimpleTrivia<shaderlab> {
        public record Line : Comment;

        public record Block : Comment;
    }

    // TODO: yeet syntax factory, use Text proeprty with pattern instead or a regex
    public static partial class SyntaxFactory {
        /// Creates appropriate comment type from the comment string. String isn't validated, just checked for initial //
        public static Comment Comment(string text) =>
            text.StartsWith("//") ? new Comment.Line { Text = text } : new Comment.Block { Text = text };
    }
}

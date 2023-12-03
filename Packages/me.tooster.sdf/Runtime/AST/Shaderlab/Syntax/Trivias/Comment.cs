using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax.Trivias {
    /// Comment text including the // or /* and */
    public abstract record Comment : SimpleTrivia<shaderlab> {
        public record Line : Comment;

        public record Block : Comment;

        public static Comment from(string text) =>
            text.Contains("\n") ? new Block { Text = $"/* {text} */" } : new Line { Text = $"// ${text}" };
    }
}

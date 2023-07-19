#nullable enable
namespace AST.Hlsl {
    public interface SyntaxNode {
        public Trivia? leadingTrivia { get; set; }
    }
}

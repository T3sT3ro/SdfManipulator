#nullable enable
namespace AST.Syntax {
    public interface ISyntaxNodeOrToken<TNode, TBase>
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase> {
        TNode? Parent { get; }
    }
}

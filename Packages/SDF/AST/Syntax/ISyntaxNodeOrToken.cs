namespace AST.Syntax {
    public interface ISyntaxNodeOrToken<out TNode, TBase>
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase> {
        TNode Parent { get; }
    }
}

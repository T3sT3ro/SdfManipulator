namespace AST.Syntax {
    public abstract class SyntaxTree<TNode, TBase>
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase> {
        public abstract string Text { get; }
        public abstract TNode  Root { get; }
    }
}

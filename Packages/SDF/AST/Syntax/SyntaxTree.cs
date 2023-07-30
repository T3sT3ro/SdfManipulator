namespace AST.Syntax {
    public abstract class SyntaxTree<TNode, TBase>
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase> {
        protected SyntaxTree(TNode root) { Root = root; }
        public TNode Root { get; }
    }
}

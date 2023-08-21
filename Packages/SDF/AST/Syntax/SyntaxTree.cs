namespace AST.Syntax {
    public abstract record SyntaxTree<TNode, TBase>(TNode Root)
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase> {
        public override string ToString() => Root.ToString();
    }
}

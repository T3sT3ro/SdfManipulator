namespace AST.Syntax {
    public interface ISyntaxTree {
        public string BuildText();
    }

    public abstract record SyntaxTree<TNode, TBase>(TNode Root) : ISyntaxTree
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase> {
        
        public string BuildText() => Root.BuildText();
    }
}

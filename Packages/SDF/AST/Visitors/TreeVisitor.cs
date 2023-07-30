using AST.Syntax;

namespace AST.Visitors {
    public abstract class TreeVisitor<TResult> {
        public virtual TResult Visit<Ttree, TNode, TBase>(Ttree tree, bool visitTrivia = false, bool visitTokens = false)
            where Ttree : SyntaxTree<TNode, TBase>
            where TNode : SyntaxNode<TNode, TBase>, TBase
            where TBase : ISyntaxNodeOrToken<TNode, TBase> {

            var node = tree.Root;
            {
                
            }
        }
    }
}

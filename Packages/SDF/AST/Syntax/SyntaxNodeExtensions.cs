using System.Collections.Generic;
using System.Linq;

namespace AST.Syntax {
    public static class SyntaxNodeExtensions {
        public static IEnumerable<TNode> DescendantNodes<TNode, TBase>(this TNode root)
            where TNode : SyntaxNode<TNode, TBase>, TBase
            where TBase : ISyntaxNodeOrToken<TNode, TBase> {
            return root.DescendantNodesAndSelf<TNode, TBase>().Skip(1);
        }

        public static IEnumerable<TNode> DescendantNodesAndSelf<TNode, TBase>(this TNode root)
            where TNode : SyntaxNode<TNode, TBase>, TBase
            where TBase : ISyntaxNodeOrToken<TNode, TBase> {
            var stack = new Stack<TNode>();
            stack.Push(root);

            while (stack.Count > 0) {
                var current = stack.Pop();
                yield return current;

                foreach (var child in current.ChildNodes.Reverse()) {
                    if (child is TNode c)
                        stack.Push(c);
                }
            }
        }

        public static IEnumerable<TBase> DescendantNodesAndTokens<TNode, TBase>(this TNode root) 
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase> {
            var stack = new Stack<TNode>();
            stack.Push(root);

            while (stack.Count > 0) {
                var current = stack.Pop();
                yield return current;

                foreach (var child in current.ChildNodes.Reverse()) {
                    if (child is TNode c)
                        stack.Push(c);
                }
            }
        }
    }
}

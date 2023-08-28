using System.Collections.Generic;
using System.Linq;

namespace AST.Syntax {
    public static class SyntaxNodeExtensions {
        public static IEnumerable<Syntax<Lang>> DescendantNodes<Lang>(this Syntax<Lang> root) =>
            root.DescendantNodesAndSelf().Skip(1);

        public static IEnumerable<Syntax<Lang>> DescendantNodesAndSelf<Lang>(this Syntax<Lang> root) {
            var stack = new Stack<Syntax<Lang>>();
            stack.Push(root);

            while (stack.Count > 0) {
                var current = stack.Pop();
                yield return current;

                foreach (var child in current.ChildNodes.Reverse()) 
                    stack.Push(child);
            }
        }

        public static IEnumerable<SyntaxOrToken<Lang>> DescendantNodesAndTokens<Lang>(this Syntax<Lang> root) {
            var stack = new Stack<SyntaxOrToken<Lang>>();
            stack.Push(root);

            while (stack.Count > 0) {
                var current = stack.Pop();
                yield return current;

                if (current is not Syntax<Lang> node) continue;

                foreach (var child in node.ChildNodesAndTokens.Reverse())
                    stack.Push(child);
            }
        }
    }
}

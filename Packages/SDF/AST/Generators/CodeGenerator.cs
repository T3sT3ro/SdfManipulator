using System.CodeDom.Compiler;
using System.IO;
using AST.Syntax;

namespace AST.Generators {
    public class CodeGenerator {
        public static string Generate<Ttree, TNode, TBase>(Ttree tree)
            where Ttree : SyntaxTree<TNode, TBase>
            where TNode : SyntaxNode<TNode, TBase>, TBase
            where TBase : ISyntaxNodeOrToken<TNode, TBase> {
            var writer = new IndentedTextWriter(new StringWriter(), "    ");

            
        }
    }
}

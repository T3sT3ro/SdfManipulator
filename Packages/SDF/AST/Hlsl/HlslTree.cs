using AST.Syntax;

namespace AST.Hlsl {
    public class HlslTree : SyntaxTree<HlslSyntax, HlslSyntaxOrToken> {
        public HlslTree(HlslSyntax root) : base(root) { }
    }
}

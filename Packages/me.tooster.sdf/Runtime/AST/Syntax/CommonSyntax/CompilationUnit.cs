using System.Collections.Generic;
using System.Text;
namespace me.tooster.sdf.AST.Syntax.CommonSyntax {
    public partial record CompilationUnit<Lang> : Statement<Lang> {
        public SyntaxList<Lang, Statement<Lang>> topLevelStatements { get; init; }
        public EofToken<Lang>                    eofToken           { get; init; }

        public override IReadOnlyList<SyntaxOrToken<Lang>> ChildNodesAndTokens()
            => new SyntaxOrToken<Lang>[] { topLevelStatements, eofToken };

        internal override void Accept(Visitor<Lang> visitor, Anchor parent) => visitor.Visit(Anchor.New(this, parent));

        internal override TR Accept<TR>(Visitor<Lang, TR> visitor, Anchor parent) where TR : default
            => visitor.Visit(Anchor.New(this, parent));

        public override string ToString() => WriteTo(new StringBuilder()).ToString();
    }
}

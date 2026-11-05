#nullable enable

namespace me.tooster.sdf.AST.Syntax {
    public abstract record SyntaxOrToken<Lang> : Tree<Lang>.Node {
        public override string ToString() => base.ToString();
    }
}

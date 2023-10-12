#nullable enable
using System.Text;

namespace me.tooster.sdf.AST.Syntax {
    public abstract record SyntaxOrToken<Lang> : Tree<Lang>.Node {
        
        public override string ToString()    => base.ToString();
        public override int    GetHashCode() => base.GetHashCode();
    }
}

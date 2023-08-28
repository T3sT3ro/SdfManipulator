#nullable enable
using System.Text;

namespace AST.Syntax {
    public abstract record SyntaxOrToken<Lang> {
        public Syntax<Lang>? Parent { get; init; } // TODO: fix visibility
        public abstract void          WriteTo(StringBuilder sb);

        public string BuildText() {
            var sb = new StringBuilder();
            WriteTo(sb);
            return sb.ToString();
        }
    }
}

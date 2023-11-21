#nullable enable
using System.Text;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST {
    public record Tree<Lang>(Syntax<Lang>? Root = null) {
        public string Text => Root?.Text ?? "";

        public abstract record Node {
            /// <summary>
            /// Writes data to some string builder.
            /// </summary>
            /// <param name="sb"></param>
            /// <returns></returns>
            public abstract StringBuilder WriteTo(StringBuilder sb);

            /// <summary>
            /// Returns string representation of this tree node.
            /// </summary>
            /// <returns></returns>
            public virtual string Text => WriteTo(new StringBuilder()).ToString();
        }
    }
}

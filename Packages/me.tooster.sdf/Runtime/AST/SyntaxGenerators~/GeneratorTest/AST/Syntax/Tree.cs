#nullable enable
using System.Text;

namespace me.tooster.sdf.AST.Syntax {
    public interface ITree { }

    public record Tree<Lang>(Syntax<Lang>? Root = null) : ITree {
        public override string ToString() => Root?.ToString() ?? "";

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
            public override string ToString() => WriteTo(new StringBuilder()).ToString();
        }
    }
}

#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace me.tooster.sdf.AST.Syntax {
    /// <summary>
    /// Internal syntax tree node. It doesn't contain text of it's own, it is an abstraction over syntax
    /// </summary>
    /// <typeparam name="Lang">marker interface of the language, makes language nodes distinct</typeparam>
    public abstract record Syntax<Lang> : SyntaxOrToken<Lang> {
        public Syntax() { }

        public abstract IReadOnlyList<SyntaxOrToken<Lang>> ChildNodesAndTokens { get; }
        public          IReadOnlyList<Syntax<Lang>> ChildNodes => ChildNodesAndTokens.OfType<Syntax<Lang>>().ToList();


        public override StringBuilder WriteTo(StringBuilder sb) {
            foreach (var child in ChildNodesAndTokens)
                child.WriteTo(sb);
            return sb;
        }

        public override string ToString() => base.ToString();
    }
}

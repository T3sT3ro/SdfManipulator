using System;
using System.Collections.Generic;
using System.Text;

namespace AST.Syntax {
    public record InjectedLanguage<Lang, InjectedLang>(Tree<InjectedLang> tree) : Syntax<Lang> {
        public override IReadOnlyList<SyntaxOrToken<Lang>> ChildNodesAndTokens =>
            Array.Empty<SyntaxOrToken<Lang>>();

        public override StringBuilder WriteTo(StringBuilder sb) => tree.Root.WriteTo(sb);
    }
}
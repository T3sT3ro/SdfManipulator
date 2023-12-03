#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace me.tooster.sdf.AST.Syntax {
    public record InjectedLanguage<Lang, InjectedLang>(Tree<InjectedLang>? tree) : Syntax<Lang> {
        public override IReadOnlyList<SyntaxOrToken<Lang>> ChildNodesAndTokens => Array.Empty<SyntaxOrToken<Lang>>();

        public override StringBuilder WriteTo(StringBuilder sb)             => tree?.Root?.WriteTo(sb) ?? sb;
        
        /// injected languages require their own formatter and their own override
        public override Syntax<Lang>? MapWith(Mapper<Lang> mapper, Anchor? thisAnchor = null) => this;
    }
}

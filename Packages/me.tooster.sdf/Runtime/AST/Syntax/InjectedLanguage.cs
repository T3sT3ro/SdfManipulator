#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace me.tooster.sdf.AST.Syntax {
    [Syntax] public partial record InjectedLanguage<Lang, InjectedLang>(Tree<InjectedLang>? tree) : Syntax<Lang> {
        public override IReadOnlyList<SyntaxOrToken<Lang>> ChildNodesAndTokens =>
            Array.Empty<SyntaxOrToken<Lang>>();

        public override StringBuilder WriteTo(StringBuilder sb) => tree?.Root?.WriteTo(sb) ?? sb;

        public InjectedLanguage() : this((Tree<InjectedLang>?)null) { }

        public override Syntax<Lang> MapWith(Mapper<Lang> mapper) => mapper.Map((dynamic)this);
    }
}

#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace me.tooster.sdf.AST.Syntax.CommonSyntax {
    public record InjectedLanguage<Lang, InjectedLang>(Tree<InjectedLang>? tree) : Syntax<Lang> {
        public override IReadOnlyList<SyntaxOrToken<Lang>> ChildNodesAndTokens() => Array.Empty<SyntaxOrToken<Lang>>();

        public override StringBuilder WriteTo(StringBuilder sb) => tree?.Root?.WriteTo(sb) ?? sb;

        internal override void Accept(Visitor<Lang> visitor, Anchor? parent) => visitor.Visit(Anchor.New(this, parent));

        internal override R? Accept<R>(Visitor<Lang, R> visitor, Anchor? parent) where R : default =>
            visitor.Visit(Anchor.New(this, parent));
        
        public override string ToString() => WriteTo(new StringBuilder()).ToString();
    }
}

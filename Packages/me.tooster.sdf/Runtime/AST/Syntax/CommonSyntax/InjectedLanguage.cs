#nullable enable
using System.Collections.Generic;
using System.Text;
namespace me.tooster.sdf.AST.Syntax.CommonSyntax {
    public record InjectedLanguage<Lang, InjectedLang>(Tree<InjectedLang>? tree) : Syntax<Lang> {
        /// virtual start token for applying leading formatting context without relying on attaching trivia to injected tree 
        public InjectedLanguageStartToken<Lang> injectedLanguageStartToken { get; init; } = new();
        /// virtual end token for applying formatting trivia just after the injected context without attaching trivia to injected tree
        public InjectedLanguageEndToken<Lang> injectedLanguageEndToken { get; init; } = new();

        public override IReadOnlyList<SyntaxOrToken<Lang>> ChildNodesAndTokens()
            => new Token<Lang>[] { injectedLanguageStartToken, injectedLanguageEndToken };

        public override StringBuilder WriteTo(StringBuilder sb) {
            sb.Append(injectedLanguageStartToken);
            tree?.Root?.WriteTo(sb);
            sb.Append(injectedLanguageEndToken);
            return sb;
        }

        internal override void Accept(Visitor<Lang> visitor, Anchor? parent) => visitor.Visit(Anchor.New(this, parent));

        internal override R? Accept<R>(Visitor<Lang, R> visitor, Anchor? parent) where R : default
            => visitor.Visit(Anchor.New(this, parent));

        public override string ToString() => WriteTo(new StringBuilder()).ToString();
    }
}

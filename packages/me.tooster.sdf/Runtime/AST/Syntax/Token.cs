#nullable enable
using System.Text;
namespace me.tooster.sdf.AST.Syntax {
    public abstract record Token<Lang> : SyntaxOrToken<Lang> {
        public TriviaList<Lang> LeadingTriviaList { get; init; } = TriviaList<Lang>.Empty;
        // public TriviaList<Lang> TrailingTriviaList { get; init; } = TriviaList<Lang>.Empty;

        public abstract string Text { get; }

        internal override void Accept(Visitor<Lang> visitor, Anchor? parent) => visitor.Visit(Anchor.New(this, parent));

        internal override R? Accept<R>(Visitor<Lang, R> visitor, Anchor? parent) where R : default
            => visitor.Visit(Anchor.New(this, parent));

        public override StringBuilder WriteTo(StringBuilder sb) {
            LeadingTriviaList.WriteTo(sb);
            sb.Append(Text);
            // TrailingTriviaList.WriteTo(sb);
            return sb;
        }

        public override string ToString() => base.ToString();
    }
}

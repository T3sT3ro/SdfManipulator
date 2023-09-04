using System.Text;

namespace me.tooster.sdf.AST.Syntax {
    public abstract record Token<Lang> : SyntaxOrToken<Lang> {
        private readonly TriviaList<Lang> _leadingTriviaList  = new();
        private readonly TriviaList<Lang> _trailingTriviaList = new();
        public abstract  string           Text { get; }

        public TriviaList<Lang> LeadingTriviaList {
            get => _leadingTriviaList;
            init => _leadingTriviaList = new(value with { Token = this });
        }

        public TriviaList<Lang> TrailingTriviaList {
            get => _trailingTriviaList;
            init => _trailingTriviaList = new(value with { Token = this });
        }

        public override StringBuilder WriteTo(StringBuilder sb) {
            LeadingTriviaList.WriteTo(sb);
            sb.Append(Text);
            TrailingTriviaList.WriteTo(sb);
            return sb;
        }

        public override string ToString() => base.ToString();
    }
}

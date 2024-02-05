#nullable enable
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace me.tooster.sdf.AST.Syntax {
    
    public abstract record Token<Lang> : SyntaxOrToken<Lang> {
        public TriviaList<Lang> LeadingTriviaList  { get; init; } = TriviaList<Lang>.Empty;
        public TriviaList<Lang> TrailingTriviaList { get; init; } = TriviaList<Lang>.Empty;

        public abstract string Text { get; }

        internal override void Accept(Visitor<Lang> visitor, Anchor? a) => visitor.Visit(Anchor.New(this, a?.Parent));

        internal override R? Accept<R>(Visitor<Lang, R> visitor, Anchor? a) where R : default =>
            visitor.Visit(Anchor.New(this, a?.Parent));

        public override StringBuilder WriteTo(StringBuilder sb) {
            LeadingTriviaList.WriteTo(sb);
            sb.Append(Text);
            TrailingTriviaList.WriteTo(sb);
            return sb;
        }

        public override string ToString() => base.ToString();
    }

    public abstract record ValidatedToken<Lang> : Token<Lang> {
        private readonly string rawText = "";

        public override string Text => rawText;

        protected abstract Regex Pattern { get; }

        /// <summary>Creates new token by validating the text.</summary>
        /// <exception cref="ArgumentException">if text doesn't match pattern</exception>
        public virtual string ValidatedText {
            init {
                if (!Pattern.IsMatch(value))
                    throw new ArgumentException($"Token text: {value} doesn't match pattern: {Pattern}");

                rawText = value;
            }
        }

        // for internal usage to bypass regex match with direct value assignment
        protected string TextUnsafe {
            init => rawText = value;
        }
        
        public override string ToString() => base.ToString();
    }

    public abstract record Literal<Lang> : ValidatedToken<Lang> {
        public override string ToString() => base.ToString();
    }
}

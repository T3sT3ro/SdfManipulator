#nullable enable
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace me.tooster.sdf.AST.Syntax {
    // TODO: think of moving all tokens inside Tokens namespace and adjust names accordingly (PlusToken -> Tokens.Plus)
    public abstract record Token<Lang> : SyntaxOrToken<Lang> {
        public TriviaList<Lang>? LeadingTriviaList  { get; init; }
        public TriviaList<Lang>? TrailingTriviaList { get; init; }

        public override StringBuilder WriteTo(StringBuilder sb) {
            LeadingTriviaList?.WriteTo(sb);
            sb.Append(FullText);
            TrailingTriviaList?.WriteTo(sb);
            return sb;
        }

        internal override void Accept(Visitor<Lang> visitor, Anchor parent) => visitor.Visit(Anchor.New(this, parent));

        internal override R? Accept<R>(Visitor<Lang, R> visitor, Anchor parent) where R : default =>
            visitor.Visit(Anchor.New(this, parent));
    }

    public abstract record ValidatedToken<Lang> : Token<Lang> {
        private readonly string validatedText;

        public override string FullText => validatedText;

        protected abstract Regex Pattern { get; }

        /// <summary>Creates new token by validating the text.</summary>
        /// <exception cref="ArgumentException">if text doesn't match pattern</exception>
        public virtual string ValidatedText {
            init {
                if (!Pattern.IsMatch(value))
                    throw new ArgumentException($"Token text: {value} doesn't match pattern: {Pattern}");

                validatedText = value;
            }
        }

        protected string TextUnsafe {
            init => validatedText = value;
        }
    }

    public abstract record Literal<Lang> : ValidatedToken<Lang>;
}
